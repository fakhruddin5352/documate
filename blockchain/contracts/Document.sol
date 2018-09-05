pragma solidity ^0.4.2;

import "./Publisher.sol";

contract Document {

    struct Presentation {
        address to;
        uint256 at;
    }

    bytes32 public hash;
    address public owner;
    string public name;  
    bool public published;
    bool public issued;
    uint256 issuedAt;
    address public issuedTo;
    mapping(address => Presentation[]) presentations;
    mapping(address => uint) presentationsTo;

    Publisher publisher;

    constructor(address publisher_, address owner_, string name_, bytes32 hash_ ) public {
        publisher = Publisher(publisher_);
        name = name_;
        owner = owner_;
        hash = hash_;
    }

    function issue(address to) public {
        require(!published, "document_is_published");
        require(msg.sender == owner, "issuer_is_not_owner");
        require(to != owner, "issuee_is_owner");
        
        issuedTo = to;
        issued = true;
    }

    function revokeIssuance() public {
        require(owner == msg.sender, "revoker_is_not_owner");
        if (issued) {
            for (uint i = 0; i < presentations[issuedTo].length; i++) {
                Presentation storage presentation = presentations[issuedTo][i];
                if (presentationsTo[presentation.to] > 0)
                    presentationsTo[presentation.to]--;
            }
            presentations[issuedTo].length = 0;
            issued = false;
            delete issuedTo;
        }
    }

    function present(address[] to) public {
        require(msg.sender == owner || issuedTo == msg.sender, "document_is_not_issued_or_owned");

        if (to.length == 0 )
            return;
    
        for (uint i = 0; i<to.length; i++) {
            bool exists = false;
            Presentation[] storage presentedTo = presentations[msg.sender];
            for (uint j = 0; j<presentedTo.length; j++) {
                if (presentedTo[j].to == to[i] ) {
                    exists = true;
                    break;
                }
            }
            if (!exists) {
                presentationsTo[to[i]]++;
                presentations[msg.sender].push(Presentation(to[i],now));
            }
        }
    }

    function revokePresentation(address[] to) public {
        for (uint i = 0; i<to.length; i++) {
            Presentation[] storage presentedTo = presentations[msg.sender];
            for (uint j = 0; j<presentedTo.length; j++) {
                if (presentedTo[j].to == to[i]) {
                    presentedTo[j] = presentedTo[presentedTo.length-1];
                    presentationsTo[to[i]]--;
                    presentedTo.length--;
                    break;
                }
            }
          
        }   
    }

    function presentedToByAnyone(address sender) public view returns (bool) {
        return presentationsTo[sender] > 0;
    }

    function presented() public view returns (bool) {
        return presentedBy(msg.sender);
    }
 
    function presentedBy(address sender) public view returns  (bool) {
        Presentation[] storage ps = presentations[sender];
        return ps.length > 0;
    }

    function presentedTo() public view returns (address[]) {
        return presentedToBy(msg.sender);
    }

    function presentedToBy(address sender) public view returns (address[]) {
        Presentation[] storage ps = presentations[sender];
        address[] memory to = new address[](ps.length);
        for (uint j = 0; j<ps.length; j++) {
            to[j] = ps[j].to;
        }
        return to;
    }

    modifier cannotBeSelf(address[] to) {
        for (uint i = 0; i < to.length; i++) {
            require(to[i] != msg.sender, "publishee_is_owner");
        }
        _;
    }

    function publish(address[] to)  public cannotBeSelf(to) {
        require(!issued, "document_is_issued");
        require(!presented(), "document_is_presented");
        require(msg.sender == owner || publisher.isDocumentPublishedTo(address(this), msg.sender), "document_is_not_issued_or_owned");

        if (to.length == 0)
            return;
        
        published = true;
        publisher.publish(address(this), msg.sender, to);
    }

    function canView(address by) public view returns (bool) {
        return issuedTo == by || owner == by || presentedToByAnyone(by) || publisher.isDocumentPublishedTo(this,by);
    }

}