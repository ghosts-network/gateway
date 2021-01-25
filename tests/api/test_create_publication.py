from src.api import NewsFeedApi

class TestCreatePublication(NewsFeedApi):
  def test_create_publication(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    resp = self.post_publication({'content': 'My first publication #awesome'})
    assert resp.status_code == 201

    resp_body = resp.json()
    assert 'id' in resp_body
    assert resp_body['content'] == 'My first publication #awesome'
    assert resp_body['comments']['totalCount'] == 0
    assert len(resp_body['comments']['topComments']) == 0
    assert resp_body['reactions']['totalCount'] == 0
    assert len(resp_body['reactions']['reactions']) == 0

  def test_create_publication_with_empty_body(self):
    # login
    self.login_as('bob', 'bob')
    resp = self.post_publication({})
    resp_body = resp.json()

    assert resp.status_code == 400
    assert 'Content' in resp_body['errors']

  def test_create_publication_unauthorized(self):
    # create publication
    resp = self.post_publication({'content': 'My first publication #awesome'})
    assert resp.status_code == 401
