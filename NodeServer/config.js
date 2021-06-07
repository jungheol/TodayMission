module.exports = {
  host     : '***************.rds.amazonaws.com',
  user     : 'username',
  password : 'password',
  port     : 3306,
  database : 'dbname',
  connectionLimit : 10
  connectionLimit : 10,
  waitForConnections: true,
  multipleStatements: true
};