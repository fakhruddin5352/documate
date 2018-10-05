pragma solidity ^0.4.24;

import "./Sharer.sol";

contract Document {

    struct Presentation {
        address to;
        uint256 at;
    }

    bytes32 public hash;
    address public owner;
    string public name;  

    Sharer publisher;
    Sharer issuer;
    Sharer presenter;

    constructor(address publisher_, address presenter_, address issuer_, address owner_, string name_, bytes32 hash_ ) public {
        publisher = Sharer(publisher_);
        presenter = Sharer(presenter_);
        issuer = Sharer(issuer_);
        name = name_;
        owner = owner_;
        hash = hash_;
    }

    function issue(address to) public {
        require(msg.sender == owner, "issuer_is_not_owner");
        require(to != owner, "issuee_is_owner");
        require(!publisher.isDocumentSharedBy(this, msg.sender), "document_is_published");
        address[] memory tos = new address[](1);
        tos[0] = to;
        issuer.share(this, msg.sender, tos);
    }




    function revokeIssuance() public {
        address to; 
        address by; 
        uint when; 
        address document;

        uint[] memory issued = issuer.getDocumentSharesBy(this, msg.sender);
        for (uint i = 0; i < issued.length; i++ ) {
            (document, by, to, when) = issuer.getShare(issued[i]);
            presenter.revokeSharesBy(this, to);
            issuer.revokeShare(this, msg.sender, to);
        }
    }

    function isIssued() public view returns (bool) {
        return issuer.isDocumentShared(this);
    }

    function getIssuedTo() public view returns (address) {
        address to; 
        address by; 
        uint when; 
        address document;

        uint[] memory shares = issuer.getDocumentSharesBy(this, owner);
        if (shares.length > 0)
        {
            (document, by, to, when) = issuer.getShare(shares[0]);
            return to;            
        }
        return 0;
    }
    ////////////////////////////////////
    function present(address[] to) public {
        require(msg.sender == owner || issuer.isShared(this, owner, msg.sender), "document_is_not_issued_or_owned");
        if (to.length == 0 )
            return;            
        presenter.share(this, msg.sender, to);
    }

    function revokePresentation(address[] to) public {        
        for (uint i = 0; i<to.length; i++) {
            presenter.revokeShare(this, msg.sender, to[i]);          
        }   
    }

    function isPresented() public view returns (bool) {
        return presenter.isDocumentShared(this);
    }

    function isPresentedBy(address by) public view returns  (bool) {
        return presenter.isDocumentSharedBy(this, by);
    }
    function getPresentedBy(address by) public view returns (address[]) {
        address to; 
        address _; 
        uint when; 
        address document;
        uint[] memory shares = presenter.getDocumentSharesBy(this, by);
        address[] memory tos = new address[](shares.length);
        for (uint i = 0; i < shares.length; i++) {
            (document, _, to, when) = presenter.getShare(shares[i]);
            tos[i] = to;
        }
        return tos;
    }
    function isPresentedByMe() public view returns (bool) {
        return presenter.isDocumentSharedBy(this, msg.sender);
    }
    function getPresentedByMe() public view returns (address[]) {
        return getPresentedBy(msg.sender);
    }

    function isPresentedTo(address to) public view returns (bool) {
        return presenter.isDocumentSharedTo(this, to);
    }
    function isPresentedToByMe(address to) public view returns (bool) {
        return isPresentedByTo(msg.sender, to);
    }
    function isPresentedByTo(address by, address to) public view returns (bool) {
        return presenter.isShared(this, by, to);
    }

    function publish(address[] to)  public cannotBeSelf(to) {
        require(!issuer.isDocumentShared(this), "document_is_issued");
        require(!isPresented(), "document_is_presented");
        require(msg.sender == owner || publisher.isDocumentSharedTo(address(this), msg.sender), "document_is_not_issued_or_owned");

        if (to.length == 0)
            return;        
        publisher.share(address(this), msg.sender, to);
    }

    function isPublished() public view returns (bool) {
        return publisher.isDocumentShared(this);
    }

    function isPublishedBy(address by) public view returns  (bool) {
        return publisher.isDocumentSharedBy(this, by);
    }
    function getPublishedBy(address by) public view returns (address[]) {
        address to; 
        address _; 
        uint when; 
        address document;
        uint[] memory shares = publisher.getDocumentSharesBy(this, by);
        address[] memory tos = new address[](shares.length);
        for (uint i = 0; i < shares.length; i++) {
            (document, _, to, when) = publisher.getShare(shares[i]);
            tos[i] = to;
        }
        return tos;
    }
    function isPublishedByMe() public view returns (bool) {
        return publisher.isDocumentSharedBy(this, msg.sender);
    }
    function getPublishedByMe() public view returns (address[]) {
        return getPublishedBy(msg.sender);
    }

    function isPublishedTo(address to) public view returns (bool) {
        return publisher.isDocumentSharedTo(this, to);
    }
    function isPublishedToByMe(address to) public view returns (bool) {
        return isPublishedByTo(msg.sender, to);
    }
    function isPublishedByTo(address by, address to) public view returns (bool) {
        return publisher.isShared(this, by, to);
    }



    function canView(address addr) public view returns (bool) {
        return owner == addr
            || issuer.isDocumentSharedTo(this, addr) 
            || presenter.isDocumentSharedTo(this, addr) 
            || publisher.isDocumentSharedTo(this, addr);
    }


    modifier cannotBeSelf(address[] to) {
        for (uint i = 0; i < to.length; i++) {
            require(to[i] != msg.sender, "publishee_is_owner");
        }
        _;
    }



}