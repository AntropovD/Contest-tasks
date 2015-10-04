def subset(number):
	st = str(number)
	return [st[0:3], st[1:], st[0:2]+st[3], st[0]+st[2:]]

def inf(_min, digit):
	for x in subset(_min):		
		_min = min(_min, int(x+digit))
	return _min

st = input()
_min = int(st[0:4])
for i in range(4, len(st)):
	_min = inf(_min, st[i])
print (_min)


