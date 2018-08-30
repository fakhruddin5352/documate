pragma solidity ^0.4.2;

import "./Publisher.sol";

contract Document {
    address public owner;
    string public name;  
    bool public published;
    bool public issued;
    bool public presented;
    address public issuedTo;
    address public presentedTo;

    Publisher publisher;

    constructor(address _publisher, address _owner, string _name) public {
        publisher = Publisher(_publisher);
        name = _name;
        owner = _owner;
    }


    modifier cannotBeSelf(address[] _to) {
        for (uint i = 0; i < _to.length; i++) {
            require(_to[i] != msg.sender, "publishee_is_owner");
        }
        _;
    }

    function issue(address to) public {
        require(!published, "document_is_published");
        require(msg.sender == owner, "owner_is_not_issuer");
        require(to != owner, "issuee_is_owner");
        
        issuedTo = to;
        issued = true;
    }

    function publish(address[] _to)  public cannotBeSelf(_to) {
        publisher.publish(address(this), msg.sender, _to);
        published = true;
    }
}