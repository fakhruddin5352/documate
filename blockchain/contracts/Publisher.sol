pragma solidity ^0.4.2;

contract Publisher {
    struct Publication {
        address document;
        address publishedTo;
    }
    mapping(address => Publication[]) publications;    

    function publish(address _document, address _publishTo) public {
        Publication memory publication = Publication(_document,_publishTo);
        publications[_document].push(publication);
    }
}
