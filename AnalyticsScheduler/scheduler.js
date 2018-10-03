var CronJob = require('cron').CronJob;
const request = require('superagent');

new CronJob('0 0 * * *', function() {
  console.log('You will see this message every second');  
  request
  .get("http://13.126.8.255/tickets/analytics/update")
  .send()
  .set('Access','Allow_Service')
  .end((err,res)=>console.log(res.body));
}, null, true);