pragma solidity ^0.4.2;

import "./Publisher.sol";

contract Document {
    address public owner;
    string public name;  
    bool public published;
    bool public issued;
    bool public presented;
    address public issuedTo;
    address[] public presentedTo;

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

    function issue(address _to) public {
        require(!published, "document_is_published");
        require(msg.sender == owner, "owner_is_not_issuer");
        require(_to != owner, "issuee_is_owner");
        
        issuedTo = _to;
        issued = true;
    }

    function present(address[] _to) public {
        require(msg.sender == owner || issuedTo == msg.sender, "document_is_not_issued_or_owned");
        presented = true;
        for (uint i = 0; i<_to.length; i++) {
            bool exists = false;
            for (uint j = 0; j<presentedTo.length; j++) {
                if (presentedTo[j] == _to[i]) {
                    exists = true;
                    break;
                }
            }
            if (!exists) {
                presentedTo.push(_to[i]);
            }
        }
    }

    function getPresentedTo() public view returns (address[]) {
        return presentedTo;
    }

    function publish(address[] _to)  public cannotBeSelf(_to) {
        publisher.publish(address(this), msg.sender, _to);
        published = true;
    }
}