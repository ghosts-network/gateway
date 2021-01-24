from src.api import NewsFeedApi

class TestGetComments(NewsFeedApi):

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
