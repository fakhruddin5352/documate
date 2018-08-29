var Publisher = artifacts.require("./Publisher.sol");

contract('Publisher', function(accounts) {
  it("should publish document from first to second account", async () => {
    let publisher = await Publisher.deployed();
      await publisher.publish(accounts[0],accounts[0],[accounts[1]]);
      console.log(accounts);
      let published = await publisher.isDocumentPublished(1, accounts[0], accounts[1]);
      assert(published, true, "Document unable to publish");
  });
});
