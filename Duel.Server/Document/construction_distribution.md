# Distribution 

## 다 받게 하는 방법 

 - 필요한 곳은 다 받도록 한다 
 - 분산 했을 때 포워딩 필요한 곳에서 받아서 전달 

### 로컬 실행 

 - MatchReq
   - UserCase / MatchCase 

 - MatchResp 
   - UserCase / ClientCase


### 분산 실행 

 - MatchReq 
   - UserCase 
   - MasterClient : Forwards to Master server
     - MatchCase

 - MatchResp 
   - UserCase 
   - SessionCase : Forwards to Client 
     - ClientCase

## 게임 처리 

### 로컬 

 - EventGameReq 
   - UserCase 
   - Instance 

 - EventGameResp 
   - UserCase
   - ClientCase 

### 분산 

- EventGameReq 
   - UserCase 
   - GameClient 
     - InstanceId에 대해 바인딩 
     - JoinResp와 같이 초기 이벤트 수신 시 바인딩

 - EventGameResp 
   - UserCase
   - Session : 클라이언트 세션에 InstanceId로 바인딩 


## 아이디어들 정리  


### 분산 구성에 의존하지 않는 방법 

요청 : 
 - LoginCase에서 EventLoginReq를 처리하지 않음 
 - MasterClient에서 나중에 마스터로 보내도록 구성 
 - 로컬에서는 AuthCase가 직접 받아서 처리  

응답 : 

 - LoginCase에서 우선 처리 
 - ClientCase로 전달하려면 ?  
 
### 이벤트를 나누는 방법
 - 편하긴 한데 게임 쪽에 맞지 않을 수 있다. 
 - Construction을 통해 만들어 가니 나중에 조정이 필요 
 - LoginReq / MasterLoginReq, MasterLoginResp / LoginResp의 분리 

 

 

