     execution: local
        script: load-test.js
        output: -

     scenarios: (100.00%) 1 scenario, 20 max VUs, 11m5s max duration (incl. graceful stop):
              * default: Up to 20 looping VUs for 10m35s over 3 stages (gracefulRampDown: 30s, gracefulStop: 30s)


     ✓ is status 200

     █ setup

       ✓ is status 200
       ✓ login response has cookie 'session_token'

     checks.........................: 100.00% ✓ 8904      ✗ 0
     data_received..................: 1.8 GB  2.8 MB/s
     data_sent......................: 5.2 MB  8.2 kB/s
     http_req_blocked...............: avg=40.46µs min=180ns    med=250ns   max=34.54ms p(90)=551ns    p(95)=611ns
     http_req_connecting............: avg=2.36µs  min=0s       med=0s      max=2.85ms  p(90)=0s       p(95)=0s
     http_req_duration..............: avg=15.89ms min=10.36ms  med=14.71ms max=1.09s   p(90)=20.38ms  p(95)=23.13ms
       { expected_response:true }...: avg=15.89ms min=10.36ms  med=14.71ms max=1.09s   p(90)=20.38ms  p(95)=23.13ms
     http_req_failed................: 0.00%   ✓ 0         ✗ 8903
     http_req_receiving.............: avg=6.61ms  min=124.68µs med=5.76ms  max=57.63ms p(90)=9.62ms   p(95)=11.68ms
     http_req_sending...............: avg=79.14µs min=27.23µs  med=77.85µs max=1.39ms  p(90)=115.07µs p(95)=139.02µs
     http_req_tls_handshaking.......: avg=36.39µs min=0s       med=0s      max=33.3ms  p(90)=0s       p(95)=0s
     http_req_waiting...............: avg=9.2ms   min=5.52ms   med=8.25ms  max=1.09s   p(90)=11.93ms  p(95)=13.46ms
     http_reqs......................: 8903    13.982728/s
     iteration_duration.............: avg=1.01s   min=1.01s    med=1.01s   max=1.47s   p(90)=1.02s    p(95)=1.02s
     iterations.....................: 8902    13.981158/s
     vus............................: 1       min=0       max=20
     vus_max........................: 20      min=20      max=20

running (10m36.7s), 00/20 VUs, 8902 complete and 0 interrupted iterations
default ✓ [======================================] 00/20 VUs 10m35s
