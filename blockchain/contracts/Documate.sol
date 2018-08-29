pragma solidity ^0.4.24;

import "./Publisher.sol";
import "./Document.sol";

contract Documate {

    address server;
    address publisher;
    address[] documents;

    event DocumentCreated(address document);

    constructor(address _publisher, address _server) public{
        publisher = _publisher;
        server = _server;
    }

    function testSignature(bytes32 documentHash, bytes memory signature) public view returns (address){
        bytes32 message = prefixed(keccak256(abi.encodePacked(documentHash, msg.sender, address(this))));
       // return splitSignature(signature);
        return recoverSigner(message, signature);

    }
    function createDocument(string name, bytes32 documentHash, bytes memory signature) public returns (address) {

        bytes32 message = prefixed(keccak256(abi.encodePacked(documentHash, msg.sender, address(this))));
        require(recoverSigner(message, signature) == server, "Invalid server signature");

        Document document = (new Document(publisher,msg.sender,name ));
        documents.push(document);
        emit DocumentCreated(document);
        return address(document);
    }

    function splitSignature(bytes memory sig) internal pure returns (uint8 v, bytes32 r, bytes32 s)
    {
        require(sig.length == 65, "Invalid signature length");
        assembly {
            // first 32 bytes, after the length prefix.
            r := mload(add(sig, 32))
            // second 32 bytes.
            s := mload(add(sig, 64))
            // final byte (first byte of the next 32 bytes).
            v := byte(0, mload(add(sig, 96)))
        }
        return (v, r, s);
    }

    function getDocuments() public view returns (address[]) {
        return documents;
    }

    function recoverSigner(bytes32 message, bytes memory sig) internal pure returns (address)
    {
        (uint8 v, bytes32 r, bytes32 s) = splitSignature(sig);
        return ecrecover(message, v+27, r, s);
    }
    /// builds a prefixed hash to mimic the behavior of eth_sign.
    function prefixed(bytes32 hash) internal pure returns (bytes32) {
        return keccak256(abi.encodePacked("\x19Ethereum Signed Message:\n32", hash));
    }
}