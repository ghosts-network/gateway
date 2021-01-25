from src.api import NewsFeedApi

class TestDeletePublication(NewsFeedApi):
  def test_delete_publication(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # delete publication
    delete_resp = self.delete_publication(publication_id)
    assert delete_resp.status_code == 204

  def test_delete_publication_unauthorized(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # logout
    self.logout()

    # delete publication
    delete_resp = self.delete_publication(publication_id)
    assert delete_resp.status_code == 401

  def test_delete_forbidden_publication(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # login as other user
    self.login_as('alice', 'alice')

    # delete publication
    delete_resp = self.delete_publication(publication_id)
    assert delete_resp.status_code == 403

  def test_delete_nonexistent_publication(self):
    # login
    self.login_as('bob', 'bob')

    # delete publication
    delete_resp = self.delete_publication('nonexistent-id')
    assert delete_resp.status_code == 404
