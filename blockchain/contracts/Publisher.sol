pragma solidity ^0.4.24;

contract Publisher {

    struct Publication {
        address document;
        address by;
        address to;
        uint when;
    }
    Publication[] publications;
    mapping(address => uint[]) publishedToPublications;    
    mapping(address => uint[]) documentToPublications;

    event PublicationsCreated(uint[] publications);

    function getPublication(uint index) public view returns (address document, address by, address to, uint when) {
        Publication storage publication = publications[index];
        return (publication.document, publication.by, publication.to, publication.when);
    }

    function getPublicationsTo(address _to) public view returns (uint[]) {
        return publishedToPublications[_to];
    }

    function isDocumentPublishedBy(address _document, address _by) public view returns (bool) {
        //take the optimum lookup path
        uint[] storage publicationsByDocument = documentToPublications[_document];
        for (uint i = 0; i < publicationsByDocument.length; i++) {
            if (publications[publicationsByDocument[i]].by == _by)
                    return true;            
            }
        return false;
    }


    function isDocumentPublishedTo(address _document, address _to) public view returns (bool) {
        //take the optimum lookup path
        uint[] storage publicationsByPublished = publishedToPublications[_to];
        uint[] storage publicationsByDocument = documentToPublications[_document];
        if (publicationsByPublished.length > publicationsByDocument.length ) {

            for (uint i = 0; i < publicationsByDocument.length; i++) {
                if (publications[publicationsByDocument[i]].to == _to)
                     return true;            
                }
        }else {
            for (uint j = 0; j < publicationsByPublished.length; j++) {
                if ( publications[publicationsByPublished[j]].document == _document)
                     return true;            
            }
        }
        return false;
    }

    function isDocumentPublished(address _document) public view returns (bool) {
        return documentToPublications[_document].length > 0;
    }

    function isPublished(address _document, address _by, address _to) public view returns (bool) {
        //take the optimum lookup path
        uint[] storage publicationsByPublished = publishedToPublications[_to];
        uint[] storage publicationsByDocument = documentToPublications[_document];
        if (publicationsByPublished.length > publicationsByDocument.length ) {

            for (uint i = 0; i < publicationsByDocument.length; i++) {
                if ( publications[publicationsByDocument[i]].by == _by &&
                     publications[publicationsByDocument[i]].to == _to)
                     return true;            
                }
        }else {
            for (uint j = 0; j < publicationsByPublished.length; j++) {
                if ( publications[publicationsByPublished[j]].by == _by &&
                     publications[publicationsByPublished[j]].document == _document)
                     return true;            
            }
        }
        return false;
    }

    function publish(address _document, address _by, address[] _to) public  returns (uint[]){

        uint[] memory created = new uint[](_to.length);
        for (uint i = 0; i < _to.length; i++) {
            address to = _to[i];
            if (isPublished(_document, _by, to) == false){
                publications.push(Publication(_document, _by, to, now));
                uint index = publications.length-1;
                documentToPublications[_document].push(index);
                publishedToPublications[to].push(index);
                created[i] = index;
            }
        }
        emit PublicationsCreated(created);
        return created;
    }
}
