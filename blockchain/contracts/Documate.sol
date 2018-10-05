pragma solidity ^0.4.24;

import "./Document.sol";

contract Documate {

    address public serverAddress;
    address public publisherAddress;
    address public presenterAddress;
    address public issuerAddress;
    mapping(bytes32 => address) public documents;
    mapping(address => address[]) owned;

    event DocumentCreated(address document);

    constructor(address publisher, address presenter, address issuer, address server) public{
        publisherAddress = publisher;
        serverAddress = server;
        presenterAddress = presenter;
        issuerAddress = issuer;
    }

    modifier validSignature(bytes32 document, bytes memory signature) {
        address signer = getSigner(document, signature);
        require(signer==serverAddress, "Invalid server signature");
        _;
    }
    function createDocument(string name, bytes32 documentHash, bytes memory signature) public validSignature(documentHash,signature) 
         returns (address) {
        Document document = (new Document(publisherAddress,presenterAddress, issuerAddress, msg.sender,name , documentHash));
        documents[documentHash] = address(document);
        owned[msg.sender].push(address(document));
        emit DocumentCreated(document);
        return address(document);
    }

    function canViewDocument(address by, bytes32 hash) public view returns (bool) {
        Document document = Document(documents[hash]);
        return document.canView(by);
    }

    function getSigner(bytes32 document, bytes memory signature) internal view returns (address){
        bytes memory packed = abi.encodePacked(document, msg.sender, address(this));
        bytes32 hash = keccak256(packed);
        bytes32 message = prefixed(hash);
        address signer = recoverSigner(message, signature);
        return signer;

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
        if (v < 2)
            v += 27;
        return (v, r, s);
    }

    function recoverSigner(bytes32 message, bytes memory sig) internal pure returns (address)
    {
        (uint8 v, bytes32 r, bytes32 s) = splitSignature(sig);
        return ecrecover(message, v, r, s);
    }
    /// builds a prefixed hash to mimic the behavior of ethsign.
    function prefixed(bytes32 hash) internal pure returns (bytes32) {
        return keccak256(abi.encodePacked("\x19Ethereum Signed Message:\n32", hash));
    }

}