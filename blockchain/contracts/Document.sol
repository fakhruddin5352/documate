pragma solidity ^0.4.2;

import "./Publisher.sol";

contract Document {

    struct Presentation {
        address to;
        uint256 at;
    }

    address public owner;
    string public name;  
    bool public published;
    bool public issued;
    uint256 issuedAt;
    address public issuedTo;
    mapping(address => Presentation[]) presentations;

    Publisher publisher;

    constructor(address _publisher, address _owner, string _name) public {
        publisher = Publisher(_publisher);
        name = _name;
        owner = _owner;
    }

    function issue(address _to) public {
        require(!published, "document_is_published");
        require(msg.sender == owner, "issuer_is_not_owner");
        require(_to != owner, "issuee_is_owner");
        
        issuedTo = _to;
        issued = true;
    }

    function revokeIssuance() public {
        require(owner == msg.sender, "revoker_is_not_owner");
        if (issued) {
            presentations[issuedTo].length = 0;
            issued = false;
            delete issuedTo;
        }
    }

    function present(address[] _to) public {
        require(msg.sender == owner || issuedTo == msg.sender, "document_is_not_issued_or_owned");

        if (_to.length == 0 )
            return;
    
        for (uint i = 0; i<_to.length; i++) {
            bool exists = false;
            Presentation[] storage presentedTo = presentations[msg.sender];
            for (uint j = 0; j<presentedTo.length; j++) {
                if (presentedTo[j].to == _to[i] ) {
                    exists = true;
                    break;
                }
            }
            if (!exists) {
                presentations[msg.sender].push(Presentation(_to[i],now));
            }
        }
    }

    function revokePresentation(address[] _to) public {
        for (uint i = 0; i<_to.length; i++) {
            Presentation[] storage presentedTo = presentations[msg.sender];
            for (uint j = 0; j<presentedTo.length; j++) {
                if (presentedTo[j].to == _to[i]) {
                    presentedTo[j] = presentedTo[presentedTo.length-1];
                    presentedTo.length--;
                    break;
                }
            }
          
        }   
    }

    function presented() public view returns (bool) {
        return presentedBy(msg.sender);
    }
 
    function presentedBy(address _sender) public view returns  (bool) {
        Presentation[] storage ps = presentations[_sender];
        return ps.length > 0;
    }

    function presentedTo() public view returns (address[]) {
        return presentedToBy(msg.sender);
    }

    function presentedToBy(address _sender) public view returns (address[]) {
        Presentation[] storage ps = presentations[_sender];
        address[] memory to = new address[](ps.length);
        for (uint j = 0; j<ps.length; j++) {
            to[j] = ps[j].to;
        }
        return to;
    }

    modifier cannotBeSelf(address[] _to) {
        for (uint i = 0; i < _to.length; i++) {
            require(_to[i] != msg.sender, "publishee_is_owner");
        }
        _;
    }

    function publish(address[] _to)  public cannotBeSelf(_to) {
        require(!issued, "document_is_issued");
        require(!presented(), "document_is_presented");
        require(msg.sender == owner || publisher.isDocumentPublishedTo(address(this), msg.sender), "document_is_not_issued_or_owned");

        if (_to.length == 0)
            return;
        
        published = true;
        publisher.publish(address(this), msg.sender, _to);
    }
}