from src.api import NewsFeedApi

class TestPutUser(NewsFeedApi):
  def test_update_user(self):
    # login
    self.login_as('bob', 'bob')

    # update user
    update_resp = self.put_user(self.bob_id, {'gender': 'male'})

    assert update_resp.status_code == 204

  def test_update_user_unauthorized(self):
    # update user
    update_resp = self.put_user(self.bob_id, {'gender': 'male'})

    assert update_resp.status_code == 401

  def test_update_forbidden_user(self):
    # login
    self.login_as('alice', 'alice')

    # update user
    update_resp = self.put_user(self.bob_id, {'gender': 'male'})

    assert update_resp.status_code == 403
