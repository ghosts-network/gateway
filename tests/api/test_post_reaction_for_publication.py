from src.api import NewsFeedApi

class TestPostReactionForPublication(NewsFeedApi):

  def test_post_reaction(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # add reaction
    resp = self.post_reaction(publication_id, {'reaction': 1})
    resp_body = resp.json()

    assert resp.status_code == 200
    assert resp_body['totalCount'] == 1
    assert resp_body['user']['type'] == 1

  def test_post_reaction_with_nonexistent_publication(self):
    # login
    self.login_as('bob', 'bob')

    # create reaction
    resp = self.post_reaction('nonexistent-id', {'reaction': 0})

    assert resp.status_code == 400

  def test_default_reaction(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    resp = self.post_reaction(publication_id, {})
    resp_body = resp.json()

    assert resp.status_code == 200
    assert resp_body['totalCount'] == 1
    assert resp_body['user']['type'] == 0

  def test_post_reaction_unauthorized(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # logout
    self.logout()

    # add reaction
    resp = self.post_reaction(publication_id, {})

    assert resp.status_code == 401