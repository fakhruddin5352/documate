const setup = require('./setup');
let Document = artifacts.require('Document');

contract('Documate', accounts => {
    const document = web3.sha3('Test document');

    let context;
    beforeEach(async () => {
        context = await setup(accounts);
    });
    
    it('should create a document', async () => {
        var sig = context.serverSign( document);
        const name = 'Document1'
        const receipt = await context.documate.createDocument(name, document, sig);
        let documentCreateEvent = receipt.logs.find(x => x.event == 'DocumentCreated' );
        assert.exists(documentCreateEvent, 'createDocument should return DocumentCreated event in transaction log');    
        assert.exists(documentCreateEvent.args, 'DocumentCreated Event should have args property');
        assert.exists(documentCreateEvent.args.document, 'DocumentCreated event should have document property in args');

        const documentContract = Document.at(documentCreateEvent.args.document);
        let createdDocumentName = await documentContract.name.call();
        assert.equal(name, createdDocumentName,  'name of the created document should be the same as the one passed');    
    });

    it('should fail on mismatch of document and signature', async () => {
        const sig = context.serverSign(document);
        const someOtherDocument = web3.sha3('0x2');
        try
        {
            const receipt = await context.documate.createDocument('Document1', someOtherDocument, sig);
        }catch(err){
            assert(/Invalid server signature/.test(err),'createDocument should revert by throwing "Invalid server signature" on document and signature mismatch');
            return;
        }
        assert(false,'createDocument should fail on mismatch of document and signature');
    });

    it('should fail on mismatch of contract and signature', async () => {
        const sig = context.serverSign(document, _contract = accounts[2]);
        try
        {
            const receipt = await context.documate.createDocument('Document1', document, sig);
        }catch(err){
            assert(/Invalid server signature/.test(err),'createDocument should revert by throwing "Invalid server signature" on contract and signature mismatch');
            return;
        }
        assert(false,'createDocument should fail on mismatch of constract and signature');
    });


    it('should fail on mismatch of server and signature', async () => {
        const sig = context.serverSign(document, _server = accounts[2]);
        try
        {
            const receipt = await context.documate.createDocument('Document1', document, sig);            
        }catch(err){
            assert(/Invalid server signature/.test(err),'createDocument should revert by throwing "Invalid server signature" on server and signature mismatch');
            return;
        }
        assert(false,'createDocument should fail on mismatch of server and signature');
    });

    it('should fail on mismatch of owner and signature', async () => {
        const sig = context.serverSign(document, _sender = accounts[2]);
        try
        {
            const receipt = await context.documate.createDocument('Document1', document, sig);
        }catch(err){
            assert(/Invalid server signature/.test(err),'createDocument should revert by throwing "Invalid server signature" on owner and signature mismatch');
            return;
        }
        assert(false,'createDocument should fail on mismatch of owner and signature');
    });


    it('should keep track of all documents', async () => {
        const sig = context.serverSign(document );
        const receipt = await context.documate.createDocument('Document1', document, sig);
        const documentCreateEvent = receipt.logs.find(x => x.event == 'DocumentCreated' );
        const documentAddr = documentCreateEvent.args.document;
        const createdDocument = await context.documate.documents.call(0);
        assert.equal(createdDocument, documentAddr, 'documents should return the list of the created documents');
    });
});