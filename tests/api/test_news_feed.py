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

  def test_get_comments_by_publication(self):
    # login
    self.login_as('bob', 'bob')

    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    comment1_resp = self.post_comment(publication_id, {'content': 'comment1 for the first publication'})
    comment1_id = comment1_resp.json()['id']

    comment2_resp = self.post_comment(publication_id, {'content': 'comment2 for the first publication'})
    comment2_id = comment2_resp.json()['id']

    resp = self.get_comments_by_publication_id(publication_id)
    resp_body = resp.json()

    assert resp.status_code == 200
    assert resp.headers['X-TotalCount'] == '2'

    assert len(resp_body) == 2
    assert resp_body[0]['id'] == comment1_id
    assert resp_body[0]['publicationId'] == publication_id
    assert resp_body[0]['content'] == 'comment1 for the first publication'
    assert resp_body[1]['id'] == comment2_id
    assert resp_body[1]['publicationId'] == publication_id
    assert resp_body[1]['content'] == 'comment2 for the first publication'

  def test_get_comment_by_nonexistent_publication(self):
    # login
    self.login_as('bob', 'bob')

    resp = self.get_comments_by_publication_id('nonexistent-id')
    resp_body = resp.json()

    assert resp.status_code == 200
    assert resp.headers['X-TotalCount'] == '0'

    assert len(resp_body) == 0

  def test_delete_comment(self):
    # login
    self.login_as('bob', 'bob')

    # create publication with comment
    publication_resp = self.post_publication({'content': 'My first publication #awesome'})
    publication_id = publication_resp.json()['id']

    comment_resp = self.post_comment(publication_id, {'content': 'comment 2 for the first publication'})
    comment_id = comment_resp.json()['id']

    # delete comment
    delete_resp = self.delete_comment(comment_id)
    assert delete_resp.status_code == 204

  def test_delete_comment_by_nonexistent_id(self):
    # login
    self.login_as('bob', 'bob')

    # delete comment
    delete_resp = self.delete_comment('nonexistent-id')
    assert delete_resp.status_code == 404
