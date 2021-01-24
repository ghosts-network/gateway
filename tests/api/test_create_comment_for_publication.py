from src.api import NewsFeedApi

class TestCreateCommentForPublication(NewsFeedApi):

  def test_post_comment(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # add comment
    resp = self.post_comment(publication_id, {'content': 'comment for the first publication'})
    resp_body = resp.json()

    assert resp.status_code == 201
    assert 'id' in resp_body

  def test_post_comment_with_nonexistent_publication(self):
    # login
    self.login_as('bob', 'bob')

    # create comment
    resp = self.post_comment('nonexistent-id', {'content': 'comment for the first publication'})
    resp_body = resp.json()

    assert resp.status_code == 400

  def test_empty_body_comment(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    resp = self.post_comment(publication_id, {})
    resp_body = resp.json()

    assert resp.status_code == 400
    assert 'Content' in resp_body['errors']
