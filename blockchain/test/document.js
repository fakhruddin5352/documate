const Document = artifacts.require('Document');
const setup = require('./setup');

contract('Document', accounts => {

    let context;
    beforeEach(async () => {
        context = await setup(accounts);
    });


    const createDocument  = async (name) => {
        const document = web3.sha3(name);
        var sig = context.serverSign( document);
        const receipt = await context.documate.createDocument(name, document, sig);
        const documentCreateEvent = receipt.logs.find(x => x.event == 'DocumentCreated' );
        const documentContract = Document.at(documentCreateEvent.args.document);
        return documentContract;
    };

    it('should be set to issued after issuane', async () => {
        const document = await createDocument("Document1");
        const issuee = accounts[2];
        await document.issue(issuee);
        assert.equal((await document.issued.call()), true, 'document should be issued' );
        assert.equal((await document.issuedTo.call()), issuee, 'document should be issued to the provided issuer' );        
    });

    it('should issue a document if not already published', async () => {
        const document = await createDocument("Document1");
        assert.equal((await document.published.call()), false, 'document should not be published');
        const issuee = accounts[2];
        await document.issue(issuee);
        assert.equal((await document.issued.call()), true, 'document should be issued' );
        assert.equal((await document.issuedTo.call()), issuee, 'document should be issued to the provided issuer' );        
    });

    it('should fail to issue if already published', async () => {
        const document = await createDocument("Document1");
        const issuee = accounts[2];
        await document.publish([issuee]);
        assert.equal((await document.published.call()), true, 'document should be published');
        try {
            await document.issue(issuee);
        } catch(err) {
            assert(/document_is_published/.test(err),'issue should revert by throwing "document_is_published" if already published');
            return;
        }
        assert(false,'issue should fail if document already published');            
    });

    it('should fail to issue if issuer is not owner', async () => {
        const document = await createDocument("Document1");
        const issuee = accounts[2];
        try {
            await document.issue(issuee,{
                from: accounts[3]
            });
        } catch (err) {
            assert(/owner_is_not_issuer/.test(err),'issue should revert by throwing "owner_is_not_issuer" if issuer is not owner');
            return;

        }
        assert(false,'issue should fail if issuer is not owner');            
    });


    it('should fail to issue if issuee is owner', async () => {
        const document = await createDocument("Document1");
        const issuee = context.sender;
        try {
            await document.issue(issuee);
        } catch (err) {
            assert(/issuee_is_owner/.test(err),'issue should revert by throwing "issuee_is_owner" if issuee is owner');
            return;

        }
        assert(false,'issue should fail if issuee is owner');            
    });

});