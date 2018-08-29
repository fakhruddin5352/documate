pragma solidity ^0.4.2;

import "./Publisher.sol";

contract Document {
    address public owner;
    string public name;  
    bool public isPublished;
    bool public isIssued;
    bool public isPresented;
    address public issuedTo;
    address public presentedTo;

    Publisher publisher;

    constructor(address _publisher, address _owner, string _name) public {
        publisher = Publisher(_publisher);
        name = _name;
        owner = _owner;
    }

    modifier ownerOnly {
        require(msg.sender == owner, "Only owner can publish documents.");
        _;
    }

    modifier cannotBeSelf(address[] _to) {
        for (uint i = 0; i < _to.length; i++) {
            require(_to[i] != msg.sender, "Cannot publish to self");
        }
        _;
    }

    function publish(address[] _to)  public ownerOnly cannotBeSelf(_to) {
        publisher.publish(address(this), msg.sender, _to);
        isPublished = true;
    }
}