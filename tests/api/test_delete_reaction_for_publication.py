from src.api import NewsFeedApi

class TestDeleteReactionForPublication(NewsFeedApi):

  def test_delete_reaction(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # add reaction
    self.post_reaction(publication_id, {'reaction': 1})

    # remove reaction
    resp = self.delete_reaction(publication_id)
    resp_body = resp.json()

    assert resp.status_code == 200
    assert resp_body['totalCount'] == 0
    assert resp_body['user'] is None

  def test_delete_reaction_with_nonexistent_publication(self):
    # login
    self.login_as('bob', 'bob')

    # delete reaction
    resp = self.delete_reaction('nonexistent-id')

    assert resp.status_code == 400

  def test_delete_reaction_unauthorized(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # add reaction
    self.post_reaction(publication_id, {'reaction': 1})

    # logout
    self.logout()

    # remove reaction
    resp = self.delete_reaction(publication_id)

    assert resp.status_code == 401