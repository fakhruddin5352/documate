let Publisher = artifacts.require('Publisher');

contract('Publisher', function (accounts) {
  let publisher;
  const documents = ['0x1', '0x2'];

  beforeEach(async () => {
    publisher = await Publisher.new();
  });
  it('should return publication indexes on publish', async () => {
    let receipt = await publisher.publish(documents[0], accounts[0], [accounts[1]]);
    let publicationsCreatedEvent = receipt.logs.find(x => x.event == 'PublicationsCreated' );
    assert.exists(publicationsCreatedEvent, 'PublicationsCreated Event not returned in transaction log');    
    assert.exists(publicationsCreatedEvent.args, 'PublicationsCreated Event has no arguments');
    assert.exists(publicationsCreatedEvent.args.publications, 'PublicationsCreated Event does not have publications arguments');
    assert.equal(publicationsCreatedEvent.args.publications.length, 1,  'Incorrect number of publications created');
    assert.equal(publicationsCreatedEvent.args.publications[0].toNumber(), 0, 'Document not published');
  });

  it('should publish document from first to second account', async () => {
    await publisher.publish(documents[0], accounts[0], [accounts[1]]);
    let published = await publisher.isPublished.call(documents[0], accounts[0], accounts[1]);
    assert.equal(published, true, 'Document unable to publish');
  });

  it('should determine correct document publication status', async () => {
    await publisher.publish(documents[0], accounts[0], [accounts[1]]);
    let published = await publisher.isDocumentPublished.call(documents[0]);
    assert.isTrue(published, 'should return published document as true');
  });
  it('should determine correct document publication status', async () => {
    published = await publisher.isDocumentPublished.call(documents[1]);
    assert.isFalse(published, 'should return unpublished document as false');
  });



});
