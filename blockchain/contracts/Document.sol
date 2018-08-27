pragma solidity ^0.4.2;

import "./Publisher.sol";

contract Document {
    address owner;
    string name;       
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

    function publish(address _to)  public ownerOnly {
        publisher.publish(address(this), _to);
    }

    function getName() public view returns (string) {
        return name;
    }

    function getOwner() public view returns (address) {
        return owner;
    }
}


