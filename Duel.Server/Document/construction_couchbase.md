# Couchbase


## 기초 

 - CouchbaseNetClient 설치  
   - NuGet으로 검색 및 설치 

 - Couchbase Community Server 설치 
   - [관리](http://localhost:8091/index.html)
   - Administrator / nw4any 

 - NewtonSoft.Json에 의존 

## 처리기 

 - MultiThreadFlow에 이벤트로 전송 
 - UserDbCase와 같은 하나의 케이스에서 처리 
 - EventDbLoadUserReq / Resp 
   - Account와 Context에 바인딩해서 처리 
 - EventDbUpdateUserReq / Resp

## ClusterHelper 

 - [자료](http://developer.couchbase.com/documentation/server/4.0/sdks/dotnet-2.2/cluster-helper.html)

 연결 처리 등을 내부적으로 해줌. 소스를 볼 수 있으니 참고하면 됨. 

## 성능 

 - 멀티 쓰레드와 싱글 쓰레드에서 모두 매우 빠르다. 
 - 초당 5만건 정도 처리 
 - C++로 구현했던 것에 비해 훨씬 빠름 
   - 이 때 이해 못 한 부분이 있었을 것으로 보이고 
   - reflection이 필요한 부분 등에서 C#이 상당히 최적화되어 있는 것으로 보임 

## Example 

 - [Examples](https://github.com/couchbaselabs/couchbase-net-examples/tree/master/Src)
   - dynamic을 사용하지 않는 예시들. dynamic은 느리고 Rosyln을 추가 설치해야 함. 
   - 클래스를 문서화 해서 사용





