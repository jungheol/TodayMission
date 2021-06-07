var express = require('express');
var mysql = require('mysql');
var util = require('util');
var dateFormat = require('dateformat');
var config = require('../config');
var pool = mysql.createPool(config);
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', { title: 'Express' });
});

/* POST home page. */
router.post('/signup', function(req, res, next) {
  var email = req.body.email;
  var password = req.body.password;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'signup: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'SELECT * FROM user_info WHERE email = %s;',
      mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'signup: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data.toString() != "") { 
        // 존재하는 이메일
        res.send({
          'code': 1, 
          'msg': 'signup: existed.'
        });
      } else { 
        // 가입 시키는 쿼리문
        query = util.format(
          'INSERT INTO user_info (email, password, createdAt) VALUES (%s, %s, %s);',
          mysql.escape(email),
          mysql.escape(password),
          mysql.escape(dateFormat(new Date(), "yyyy-mm-dd hh:MM:ss"))
        );

        connection.query(query, function(err, data) {
          if(err) {
            res.send({
              'code': 2, 
              'msg': 'signup: query error',
              'err': err
            });
            connection.release();
            return;
          } 

          // 가입 완료
          res.send({
            'code': 0, 
            'msg': 'signup: success.',
            'data': data
          });
    
          connection.release();
        });
      }
    // connection.release();  중복 이메일 검사 if문에서 끝날 때 release 존재하지 않음.
    });

  });
});

router.post('/login', function(req, res, next) {
  var email = req.body.email;
  var password = req.body.password;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'login: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
        'SELECT * FROM user_info WHERE email = %s and password = %s and deleteYN = %s;',
      mysql.escape(email),
      mysql.escape(password),
      mysql.escape("N")
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'login: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data.toString() != "") { 
        // 이메일과 비밀번호 일치하면 로그인 성공
        res.send({
          'code': 0, 
          'msg': 'login: success.'
        });
      } else { 
        // 이메일이나 비밀번호가 일치하지 않으면 로그인 실패
        res.send({
          'code': 1, 
          'msg': 'login: failed.'
        });
      }
      connection.release();
    });

  });
});

router.post('/loadMission', function(req, res, next) {
  var createdAt = req.body.date;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'loadMission: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
        'SELECT * FROM mission WHERE DATEDIFF(%s, createdAt) < 10 ORDER BY createdAt DESC;',
      mysql.escape(createdAt),
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'loadMission: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      console.log(data);
      if(data.toString() != "") { 
        // 쿼리문 조건에 맞는 데이터가 있으면 데이터 불러오기
        res.send({
          'code': 0, 
          'msg': 'loadMission: success.',
          'data': data
        });
      } else { 
        // 조건에 맞는 데이터가 없으면 데이터 불러오기 실패
        res.send({
          'code': 1, 
          'msg': 'loadMission: failed.'
        });
      }
      connection.release();
    });

  });
});

router.post('/changePassword', function(req, res, next) {
  var email = req.body.email;
  var password = req.body.password;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'changePassword: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
        'UPDATE user_info SET password = %s WHERE email = %s;',
        mysql.escape(password),
        mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'changePassword: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data.changedRows > 0) { 
        // 비밀번호가 하나라도 바뀌면 비밀번호 변경 성공
        res.send({
          'code': 0, 
          'msg': 'changePassword: success.'
        });
      } else { 
        // 변경된 내용이 없으면 변경 실패 메시지
        res.send({
          'code': 1, 
          'msg': 'changePassword: failed.',
        });
      }
      connection.release();
    });

  });
});

  // 별명 업데이트
router.post('/updateName', function(req, res, next) {
  var email = req.body.email;
  var name = req.body.name;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'updateName: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'SELECT * FROM user_info WHERE name = %s;',
      mysql.escape(name)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'updateName: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data.toString() != "") { 
        // 존재하는 별명
        res.send({
          'code': 1, 
          'msg': 'updateName: existed.'
        });
      } else { 
        // 별명 추가시키는 쿼리문
        query = util.format(
          'UPDATE user_info SET name = %s WHERE email = %s;',
          mysql.escape(name),
          mysql.escape(email)
        );

        connection.query(query, function(err, data) {
          if(err) {
            res.send({
              'code': 2, 
              'msg': 'updateName: query error',
              'err': err
            });
            connection.release();
            return;
          } 

          // 별명 입력 완료
          res.send({
            'code': 0, 
            'msg': 'updateName: success.',
            'data': data
          });
    
          connection.release();
        });
      }
    connection.release();
    });
  });
});


  // 미션: 좋아요
router.post('/likeMission', function(req, res, next) {
    // email
    // 미션 번호

    // 1) 해당 미션에 like값을 +1
    // UPDATE mission SET like = like + 1 WHERE id = %d; 

    // 2) 해당 이메일이 like 한 미션들에 대한 정보를 저장
    // INSERT INTO likes (mission_id, email) VALUES (%d, %s);

  var email = req.body.email;
  var mission_id = req.body.mission_id;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'likeMission: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'UPDATE mission SET likes = likes + 1 WHERE id = %d;',
      mission_id
    );

    query += util.format(
      'INSERT INTO like_missions (mission_id, email) VALUES (%d, %s);',
      mission_id,
      mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'likeMission: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data[0].changedRows > 0 && data[1].insertId > 0) { 
        // 바뀐 내용이 있는지 판단하는 조건문
        res.send({
          'code': 0, 
          'msg': 'likeMission: success.'
        });
      } else { 
        // 바뀐 내용이 없을 때 에러메시지
        res.send({
          'code': 1, 
          'msg': 'likeMission: failed.',
        });
      }
      connection.release();
    });

  });
});

  // 미션: 좋아요 취소
router.post('/unlikeMission', function(req, res, next) {
    // email
    // 미션 번호

    // 1) 해당 미션에 like값을 -1
    // UPDATE mission SET like = like - 1 WHERE id = %d; 

    // 2) 해당 이메일이 like 한 미션들에 대한 정보를 삭제
    // DELETE FROM like_missions WHERE mission_id = %d and email = %s;

  var email = req.body.email;
  var mission_id = req.body.mission_id;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'unlikeMission: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'UPDATE mission SET `likes` = `likes` - 1 WHERE id = %d;',
      mission_id
    );

    query += util.format(
      'DELETE FROM like_missions WHERE mission_id = %d and email = %s;',
      mission_id,
      mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'unlikeMission: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data[0].changedRows > 0 && data[1].affectedRows > 0) { 
        // 바뀐 내용 있는지 확인해서 바꼈으면 성공
        res.send({
          'code': 0, 
          'msg': 'unlikeMission: success.'
        });
      } else { 
        // 바뀐 내용이 없을 때 에러메시지
        res.send({
          'code': 1, 
          'msg': 'unlikeMission: failed.',
        });
      }
      connection.release();
    });

  });
});

  // 댓글 목록 가져오기
router.post('/getComment', function(req, res, next) {
  // 미션 번호
  // SELECT * FROM comments WHERE mission_id = %d ORDER BY createdAt DESC;
  var email = req.body.email;
  var mission_id = req.body.mission_id;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'getComment: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'SELECT * FROM comments WHERE mission_id = %d ORDER BY createdAt DESC;',
      mission_id
    );

    query += util.format(
      'SELECT comments.id FROM comments, like_comments WHERE comments.mission_id = %d AND like_comments.email = %s AND comments.id = like_comments.comment_id;',
      mission_id,
      mysql.escape(email)
    );

    // 위 댓글에서 내가 좋아요 누른 것이 뭔지 알아내기

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'getComment: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data[0].toString() != "") { 
        // 해당 미션(번호)에 댓글 내용을 가져오는데 성공
        res.send({
          'code': 0, 
          'msg': 'getComment: success.',
          'data': data[0],
          'like': data[1]
        });
      } else { 
        // 해당 미션(번호)가 없을 때 가져오기 실패
        res.send({
          'code': 1, 
          'msg': 'getComment: failed.',
        });
      }
      connection.release();
    });
  
  });
});

  // 댓글: 좋아요
router.post('/likeComment', function(req, res, next) {
  // email
  // 미션 번호
  // 댓글 번호

  // 1) 해당 댓글에 like값을 +1
  // UPDATE comments SET like = like + 1 WHERE id = %d; 

  // 2) 해당 이메일이 like 한 댓글들에 대한 정보를 저장
  // INSERT INTO like_comments (comment_id, email) VALUES (%d, %s);

  // 3) 해당 댓글을 작성한 사람의 포인트 플러스  -> 구현해야 함.
  var email = req.body.email;
  var mission_id = req.body.mission_id;
  var comment_id = req.body.comment_id;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'likeComment: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'UPDATE comments SET likes = likes + 1 WHERE id = %d;',
      mission_id
    );
    
    query += util.format(
      'INSERT INTO like_comments (comment_id, email) VALUES (%d, %s);',
      comment_id,
      mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'likeComment: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data[0].changedRows > 0 && data[1].insertId > 0) { 
        // 바뀐 내용 있는지 확인해서 바꼈으면 성공
        res.send({
          'code': 0, 
          'msg': 'likeComment: success.'
        });
      } else { 
        // 바뀐 내용이 없을 때 에러메시지
        res.send({
          'code': 1, 
          'msg': 'likeComment: failed.',
        });
      }
      connection.release();
    });
  
  });
});

  // 댓글: 좋아요 취소
router.post('/unlikeComment', function(req, res, next) {
  // email
  // 미션 번호
  // 댓글 번호

  // 1) 해당 댓글에 like값을 -1
  // UPDATE comments SET like = like - 1 WHERE id = %d; 

  // 2) 해당 이메일이 like 한 댓글들에 대한 정보를 삭제
  // DELETE FROM like_comments WHERE comment_id = %d and email = %s;

  // 3) 해당 댓글을 작성한 사람의 포인트 마이너스  -> 구현해야 함.
  var email = req.body.email;
  var mission_id = req.body.mission_id;
  var comment_id = req.body.comment_id;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'unlikeComment: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'UPDATE comments SET likes = likes - 1 WHERE id = %d;',
      mission_id
    );
    
    query += util.format(
      'DELETE FROM like_comments WHERE comment_id = %d and email = %s;',
      comment_id,
      mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'unlikeComment: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data[0].changedRows > 0 && data[1].affectedRows > 0) { 
        // 바뀐 내용 있는지 확인해서 바꼈으면 성공
        res.send({
          'code': 0, 
          'msg': 'unlikeComment: success.'
        });
      } else { 
        // 바뀐 내용이 없을 때 에러메시지
        res.send({
          'code': 1, 
          'msg': 'unlikeComment: failed.',
        });
      }
      connection.release();
    });
  
  });
});

  // 미션: 완료
router.post('/completeMission', function(req, res, next) {
  // email
  // 미션 번호

  // 1) 해당 미션에 complete값을 +1
  // UPDATE mission SET complete = complete + 1 WHERE id = %d; 

  // 2) 해당 이메일이 complete 한 미션들에 대한 정보를 저장
  // INSERT INTO complete (mission_id, email, text, createdAt) VALUES (%d, %s, %s, %s);

  // 3) 해당 이메일의 점수를 플러스  -> 구현해야 함.
  var email = req.body.email;
  var mission_id = req.body.mission_id;
  var text = req.body.text;
  var createdAt = req.body.date;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'completeMission: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'UPDATE mission SET complete = complete + 1 WHERE id = %d;',
      mission_id
    );
    
    query += util.format(
      'INSERT INTO comments (mission_id, email, text, createdAt) VALUES (%d, %s, %s, %s);',
      mission_id,
      mysql.escape(email),
      mysql.escape(text),
      mysql.escape(dateFormat(new Date(), "yyyy-mm-dd hh:MM:ss"))
    );

    query += util.format(
      'INSERT INTO complete (mission_id, email) VALUES (%d, %s);',
      mission_id,
      mysql.escape(email)
    );

    query += util.format(
      'UPDATE user_info SET point = point + 1 WHERE email = %s;',
      mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'completeMission: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data[0].changedRows > 0 && data[1].insertId > 0 && data[2].insertId > 0) { 
        // 바뀐 내용 있는지 확인해서 바꼈으면 성공
        res.send({
          'code': 0, 
          'msg': 'completeMission: success.'
        });
      } else { 
        // 바뀐 내용이 없을 때 에러메시지
        res.send({
          'code': 1, 
          'msg': 'completeMission: failed.',
        });
      }
      connection.release();
    });
  
  });
});

  // 랭킹 가져오기
router.post('/getRank', function(req, res, next) {
  // SELECT * FROM user_info ORDER BY point DESC LIMIT 100;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'getRank: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'SELECT * FROM user_info WHERE point > 0 ORDER BY point DESC LIMIT 100;',
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'getRank: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data.toString() != "") { 
        // 점수가 있는 100개의 아이디 불러오기 성공
        res.send({
          'code': 0, 
          'msg': 'getRank: success.',
          'data': data
        });
      } else { 
        // 점수가 없을 때 가져오기 실패
        res.send({
          'code': 1, 
          'msg': 'getRank: failed.',
        });
      }
      connection.release();
    });
  
  });
});

  // 내 랭킹 가져오기
  router.post('/getMyRank', function(req, res, next) {
    // email
    // ??

  });

  // 프로필 정보 가져오기
router.post('/getProfile', function(req, res, next) {
  // email

  // 1) 해당 이메일에 대한 포인트와 가입일시
  // SELECT email, point, createdAt FROM user_info WHERE email = %s;

  // 2) 해당 이메일이 획득한 뱃지 정보
  // SELECT * FROM badges WHERE email = %s;

  var email = req.body.email;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'getProfile: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'SELECT email, point, createdAt FROM user_info WHERE email = %s;',
      mysql.escape(email)
    );

    query += util.format(
      'SELECT * FROM badges WHERE email = %s;',
      mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'getProfile: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data.toString() != "") { 
        // 프로필 정보 불러오기 성공
        res.send({
          'code': 0, 
          'msg': 'getProfile: success.'
        });
      } else { 
        // 정보가 없을 때 가져오기 실패
        res.send({
          'code': 1, 
          'msg': 'getProfile: failed.',
        });
      }
      connection.release();
    });
  
  });
});

  // 뱃지 획득
router.post('/addBadge', function(req, res, next) {
  // email
  // 뱃지 번호

  // INSERT INTO badges (badge_id, email) VALUES (%d, %s);
  var email = req.body.email;
  var badge_id = req.body.badge_id;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'addBadge: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'INSERT INTO badges (badge_id, email) VALUES (%d, %s);',
      badge_id,
      mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'addBadge: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data.insertId > 0) { 
        // 새로운 내용이 추가 되었으면 성공
        res.send({
          'code': 0, 
          'msg': 'addBadge: success.'
        });
      } else { 
        // 새로운 내용이 추가되지 않았을 때 에러메시지
        res.send({
          'code': 1, 
          'msg': 'addBadge: failed.',
        });
      }
      connection.release();
    });
  
  });
});

  // 회원탈퇴
router.post('/removeAccount', function(req, res, next) {
  // email

  // UPDATE user_info SET deleteYN = %s WHERE email = %s;
  var email = req.body.email;

  pool.getConnection(function(err, connection) {
    if(err) { 
      res.send({
        'code': 3, 
        'msg': 'removeAccount: database connection error',
        'err': err
      });
      return;
    }

    var query = util.format(
      'UPDATE user_info SET deleteYN = %s WHERE email = %s;',
      mysql.escape("Y"),
      mysql.escape(email)
    );

    connection.query(query, function(err, data) {
      if(err) {
        res.send({
          'code': 2, 
          'msg': 'removeAccount: query error',
          'err': err
        });
        connection.release();
        return;
      } 

      if(data.changedRows > 0) { 
        // 새로운 내용이 추가 되었으면 성공
        res.send({
          'code': 0, 
          'msg': 'removeAccount: success.'
        });
      } else { 
        // 새로운 내용이 추가되지 않았을 때 에러메시지
        res.send({
          'code': 1, 
          'msg': 'removeAccount: failed.',
        });
      }
      connection.release();
    });
  
  });
});

module.exports = router;
