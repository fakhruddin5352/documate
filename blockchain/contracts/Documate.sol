pragma solidity ^0.4.2;

import "./Publisher.sol";
import "./Document.sol";

contract Documate {

    address publisher;
    address[] documents;

    event LogNewDocument(address document);

    constructor() public{
        publisher = new Publisher();
    }

    function createDocument(string name) public returns (address) {
        Document document = (new Document(publisher,msg.sender,name ));
        documents.push(document);
        emit LogNewDocument(document);
        return address(document);
    }


    function getDocuments() public view returns (address[]) {
        return documents;
    }
}