pragma solidity ^0.4.24;

contract Sharer {

    struct Share {
        address document;
        address by;
        address to;
        uint when;
    }
    Share[] shares;
    uint[] empty;
    uint emptyLength;
    mapping(address => uint[]) sharedToShares;    
    mapping(address => uint[]) documentToShares;
    mapping(address => uint) sharedToSharesLength;
    mapping(address => uint) documentToSharesLength;
    

    event SharesCreated(uint[] shares);

    function revokeShare(address document, address by, address to)  public {
        uint toLength = sharedToSharesLength[to];
        uint documentLength = documentToSharesLength[document];
        uint shareIndex;
        if (toLength > documentLength ) {
            uint[] storage sharesByDocument = documentToShares[document];
            for (uint i = documentLength; i > 0; i--) {
                shareIndex = sharesByDocument[i-1];
                if (shares[shareIndex].to == to && shares[shareIndex].by == by) {
                    removeSharedToShare(shareIndex, to);
                    sharesByDocument[i-1] = sharesByDocument[documentLength-1];
                    documentLength = documentLength - 1;
                }
            }
            documentToSharesLength[document] = documentLength;
        }else {
            uint[] storage sharesTo = sharedToShares[to];
            for (uint j = toLength; j > 0; j--) {
                shareIndex = sharesTo[j-1];
                if (shares[shareIndex].to == document) {
                    removeDocumentToShare(shareIndex, document);
                    sharesTo[j-1] = sharesTo[toLength-1];
                    toLength = toLength - 1;
                }
            }
            sharedToSharesLength[to] = toLength;

        }
    }

    function revokeShares(address document) public {
        uint[] storage sharesByDocument = documentToShares[document];
        uint sharesByDocumentLength = documentToSharesLength[document];

        for (uint i = 0; i < sharesByDocumentLength; i++) {
            uint shareIndex = sharesByDocument[i];
            address to = shares[shareIndex].to;
            bool removed = removeSharedToShare(shareIndex, to);
            if (removed) {
                removeShare(shareIndex);
            }
        }
        documentToSharesLength[document] = 0;
    }

    function revokeSharesTo(address document, address to) public {
        uint toLength = sharedToSharesLength[to];
        uint documentLength = documentToSharesLength[document];
        uint shareIndex;
        if (toLength > documentLength ) {
            uint[] storage sharesByDocument = documentToShares[document];
            for (uint i = documentLength; i > 0; i--) {
                shareIndex = sharesByDocument[i-1];
                if (shares[shareIndex].to == to) {
                    removeSharedToShare(shareIndex, to);
                    sharesByDocument[i-1] = sharesByDocument[documentLength-1];
                    documentLength = documentLength - 1;
                }
            }
            documentToSharesLength[document] = documentLength;
        }else {
            uint[] storage sharesTo = sharedToShares[to];
            for (uint j = toLength; j > 0; j--) {
                shareIndex = sharesTo[j-1];
                if (shares[shareIndex].to == document) {
                    removeDocumentToShare(shareIndex, document);
                    sharesTo[j-1] = sharesTo[toLength-1];
                    toLength = toLength - 1;
                }
            }
            sharedToSharesLength[to] = toLength;

        }
    }


    function revokeSharesBy(address document, address by) public {
        uint[] storage sharesByDocument = documentToShares[document];
        uint documentLength = documentToSharesLength[document];
        for (uint i = documentLength; i > 0; i--) {
            uint shareIndex = sharesByDocument[i-1];
            if (shares[shareIndex].by == by) {
                removeSharedToShare(shareIndex, shares[shareIndex].to);
                sharesByDocument[i-1] = sharesByDocument[documentLength-1];
                documentLength = documentLength - 1;
            }
        }
        documentToSharesLength[document] = documentLength;
    }

    function getShare(uint index) public view returns (address document, address by, address to, uint when) {
        Share storage share = shares[index];
        return (share.document, share.by, share.to, share.when);
    }

    function getSharesTo(address to) public view returns (uint[]) {
        uint length = sharedToSharesLength[to];
        uint[] memory tos = new uint[](length);
        uint[] storage sharedTo = sharedToShares[to];
        for (uint i = 0; i < length; i++){
            tos[i] = sharedTo[i];
        }
        return tos;
    }

    function isDocumentSharedBy(address document, address by) public view returns (bool) {
        //take the optimum lookup path
        uint length = documentToSharesLength[document];
        uint[] storage sharesByDocument = documentToShares[document];
        for (uint i = 0; i < length; i++) {
            if (shares[sharesByDocument[i]].by == by)
                    return true;            
            }
        return false;
    }

    function getDocumentSharesBy(address document, address by) public view returns (uint[]) {
        //take the optimum lookup path
        uint length = documentToSharesLength[document];
        uint[] storage sharesByDocument = documentToShares[document];
        uint total = 0;
        for (uint i = 0; i < length; i++) {
            if (shares[sharesByDocument[i]].by == by)
                total++;
        }
        uint[] memory result = new uint[](total);
        uint k = 0;
        for (uint j = 0; j < length; j++) {
            if (shares[sharesByDocument[j]].by == by){
                result[k] = sharesByDocument[j];
                k++;
            }
        }
        return result;
    }
    


    function isDocumentSharedTo(address  document, address to) public view returns (bool) {
        //take the optimum lookup path
        uint toLength = sharedToSharesLength[to];
        uint documentLength = documentToSharesLength[document];
        if (toLength > documentLength ) {

            uint[] storage sharesByDocument = documentToShares[document];
            for (uint i = 0; i < documentLength; i++) {
                if (shares[sharesByDocument[i]].to == to)
                     return true;            
                }
        }else {
            uint[] storage sharesByShared = sharedToShares[to];
            for (uint j = 0; j < toLength; j++) {
                if ( shares[sharesByShared[j]].document == document)
                     return true;            
            }
        }
        return false;
    }

    function getDocumentSharesTo(address  document, address to) public view returns (uint[]) {
        //take the optimum lookup path
        uint toLength = sharedToSharesLength[to];
        uint documentLength = documentToSharesLength[document];
        uint total;
        uint i;
        uint j;
        uint k;
        uint[] memory result;
        if (toLength > documentLength ) {
            uint[] storage sharesByDocument = documentToShares[document];
            total = 0;
            for (i = 0; i < documentLength; i++) {
                if (shares[sharesByDocument[i]].to == to)
                    total++;
            }
            result = new uint[](total);
            k = 0;
            for (j = 0; j < documentLength; j++) {
                if (shares[sharesByDocument[j]].to == to)
                    result[k++] = sharesByDocument[j];
            }
            return result;
        }else {
            uint[] storage sharesByShared = sharedToShares[to];
            total = 0;
            for (i = 0; i < documentLength; i++) {
                if (shares[sharesByShared[i]].document == document)
                    total++;
            }
            result = new uint[](total);
            k = 0;
            for (j = 0; j < documentLength; j++) {
                if (shares[sharesByShared[j]].document == document)
                    result[k++] = sharesByShared[j];
            }
            return result;
        }
    }


    function isDocumentShared(address document) public view returns (bool) {
        return documentToSharesLength[document] > 0;
    }

    function isShared(address document, address by, address to) public view returns (bool) {
        //take the optimum lookup path
        uint toLength = sharedToSharesLength[to];
        uint documentLength = documentToSharesLength[document];
        if (toLength > documentLength ) {

            uint[] storage sharesByDocument = documentToShares[document];
            for (uint i = 0; i < documentLength; i++) {
                if ( shares[sharesByDocument[i]].by == by &&
                     shares[sharesByDocument[i]].to == to)
                     return true;            
                }
        }else {
            uint[] storage sharesByShared = sharedToShares[to];
            for (uint j = 0; j < toLength; j++) {
                if ( shares[sharesByShared[j]].by == by &&
                     shares[sharesByShared[j]].document == document)
                     return true;            
            }
        }
        return false;
    }

    function share(address document, address by, address[] to) public  returns (uint[]){

        uint[] memory created = new uint[](to.length);
        for (uint i = 0; i < to.length; i++) {
            address t = to[i];
            if (isShared(document, by, t) == false){
                uint index = addShare(document, by, t, now);
                addDocumentToShare(index, document);
                addSharedToShare(index, t);
                created[i] = index;
            }
        }
        emit SharesCreated(created);
        return created;
    }

    function removeShare(uint index) internal {
        if (emptyLength < empty.length) {
            empty[emptyLength] = index;
        }else{
            empty.push(index);
        }
        emptyLength ++;
    }
    function addShare(address document, address by, address to, uint when) internal returns (uint){
        if (emptyLength > 0) {
            uint index = empty[emptyLength-1];
            shares[index] = Share(document, by, to, when);
            emptyLength--;
            return index;
        }else{
            shares.push(Share(document, by, to, when));
            return shares.length-1;
        }
    }

    function addDocumentToShare(uint index, address document) internal {
        uint[] storage tos = documentToShares[document];
        uint length = documentToSharesLength[document];
        if (length < tos.length) {
            tos[length] = index;
        } else{
            tos.push(index);
        }
        documentToSharesLength[document] = length+1;
    }
    function removeDocumentToShare(uint index, address document) internal returns (bool){
        uint[] storage tos = documentToShares[document];
        uint length = documentToSharesLength[document];
        for (uint i = 0; i < length; i++){
            if (tos[i] == index) {
                tos[i] = tos[length];
                documentToSharesLength[document] = length-1;
                return true;
            }
        }
        return false;
    }
    function addSharedToShare(uint index, address to) internal {
        uint[] storage tos = sharedToShares[to];
        uint length = sharedToSharesLength[to];
        if (length < tos.length) {
            tos[length] = index;
        } else{
            tos.push(index);
        }
        sharedToSharesLength[to] = length+1;
    }
    function removeSharedToShare(uint index, address to) internal returns (bool) {
        uint[] storage tos = sharedToShares[to];
        uint length = sharedToSharesLength[to];
        for (uint i = 0; i < length; i++){
            if (tos[i] == index) {
                tos[i] = tos[length];
                sharedToSharesLength[to] = length-1;
                return true;
            }
        }
        return false;
    }

}
