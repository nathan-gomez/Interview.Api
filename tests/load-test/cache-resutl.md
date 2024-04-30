     execution: local
        script: load-test.js
        output: -

     scenarios: (100.00%) 1 scenario, 20 max VUs, 11m5s max duration (incl. graceful stop):
              * default: Up to 20 looping VUs for 10m35s over 3 stages (gracefulRampDown: 30s, gracefulStop: 30s)


     ✓ is status 200

     █ setup

       ✓ is status 200
       ✓ login response has cookie 'session_token'

     checks.........................: 100.00% ✓ 8922      ✗ 0
     data_received..................: 1.8 GB  2.8 MB/s
     data_sent......................: 5.2 MB  8.1 kB/s
     http_req_blocked...............: avg=38.33µs min=190ns    med=341ns   max=23.24ms p(90)=621ns    p(95)=702ns
     http_req_connecting............: avg=2.48µs  min=0s       med=0s      max=2.48ms  p(90)=0s       p(95)=0s
     http_req_duration..............: avg=14.11ms min=7.24ms   med=11.29ms max=4.39s   p(90)=17.32ms  p(95)=20.24ms
       { expected_response:true }...: avg=14.11ms min=7.24ms   med=11.29ms max=4.39s   p(90)=17.32ms  p(95)=20.24ms
     http_req_failed................: 0.00%   ✓ 0         ✗ 8921
     http_req_receiving.............: avg=7.52ms  min=122.84µs med=6.45ms  max=86.9ms  p(90)=11.43ms  p(95)=13.78ms
     http_req_sending...............: avg=80.71µs min=27.48µs  med=79.9µs  max=1.04ms  p(90)=112.28µs p(95)=134.99µs
     http_req_tls_handshaking.......: avg=34.27µs min=0s       med=0s      max=20.64ms p(90)=0s       p(95)=0s
     http_req_waiting...............: avg=6.5ms   min=2.26ms   med=4.51ms  max=4.39s   p(90)=6.71ms   p(95)=8.05ms
     http_reqs......................: 8921    13.950263/s
     iteration_duration.............: avg=1.01s   min=1s       med=1.01s   max=4.41s   p(90)=1.01s    p(95)=1.02s
     iterations.....................: 8920    13.9487/s
     vus............................: 1       min=0       max=20
     vus_max........................: 20      min=20      max=20

running (10m39.5s), 00/20 VUs, 8920 complete and 0 interrupted iterations
default ✓ [======================================] 00/20 VUs 10m35s
