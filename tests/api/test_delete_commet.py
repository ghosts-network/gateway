from src.api import NewsFeedApi

class TestDeleteComment(NewsFeedApi):

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

  def test_delete_nonexistent_comment(self):
    # login
    self.login_as('bob', 'bob')

    # delete comment
    delete_resp = self.delete_comment('nonexistent-id')
    assert delete_resp.status_code == 404
