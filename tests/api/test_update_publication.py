from src.api import NewsFeedApi

class TestUpdatePublication(NewsFeedApi):
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
