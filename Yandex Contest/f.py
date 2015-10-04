(n, L) = map(int, input().split())

res = [0, 0, 0]

for i in range(0, n):
	(team,dist) = map(int, input().split())	
	if dist==-1:
		res[team]+=1
	elif dist<=L:
		res[team]+=2
	else: 
		res[team]+=3

print ("{0}:{1}".format(res[1], res[2]))