# Auth

 - DB query processing required. 
 - Go with Couchbase 

## Event Partition 

 Login, MasterLogin 형태로 분리. 

 동일 이벤트를 사용한다면 필드 추가가 필요하다. 

 Db event들에 있는 Context와 같은 구조.

## AuthCase

 LoginPending -> Login 
 Logout 처리  
 대략적으로 구현. 
 
## UserDatabaseCase 

 ClusterHelper를 사용해서 간단 처리. 
 쉽게 잘 된다. NoSQL의 장점은 있다. 
 트랜잭션 처리만 간결하게 있으면 된다.  



