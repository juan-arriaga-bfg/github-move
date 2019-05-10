var express = require('express');
var router = express.Router();
var dataParser = require('sources/dataParser');

/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', { title: 'Analytics monitor' });
});

router.post('/hacked-analytics/*', function(req, res, next) {
    dataParser.add(req.body.data);
    res.status(200).send("OK");
});

router.post('/hacked-analytics/', function(req, res, next) {
    dataParser.add(req.body.data);
    res.status(200).send("OK");
});

router.post('/hacked-analytics', function(req, res, next) {
    dataParser.add(req.body.data);
    res.status(200).send("OK");
});

router.get('/debug', function(req, res, next) {
    var text = dataParser.debug();
    res.status(200).send(text);
});

router.get('/data', function(req, res) {
    var id = req.param('id');
    var data = dataParser.get(id);
    res.status(200).send(data);
});

module.exports = router;
