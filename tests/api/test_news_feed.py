from src.api import NewsFeedApi

class TestNewsFeed(NewsFeedApi):
  def test_correct_publication(self):
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

  def test_empty_body_publication(self):
    # login
    self.login_as('bob', 'bob')
    resp = self.post_publication({})
    resp_body = resp.json()

    assert resp.status_code == 400
    assert 'Content' in resp_body['errors']

  def test_delete_publication(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # delete publication
    delete_resp = self.delete_publication(publication_id)
    assert delete_resp.status_code == 204

  def test_update_publication_with_empty_body(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']
  
    # update publication
    resp = self.put_publication(publication_id, {})
    resp_body = resp.json()

    assert resp.status_code == 400
  
  def test_update_publication(self):
    # login
    self.login_as('bob', 'bob')

    # create publication
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    # update publication
    update_resp = self.put_publication(publication_id, {'content': 'Updated content #test #cat'})
    assert update_resp.status_code == 204

  def test_update_nonexistent_publication(self):
    # login
    self.login_as('bob', 'bob')

    # update publication
    resp = self.put_publication('nonexistent_id', {'content': 'Updated content #test #cat'})

    assert resp.status_code == 404