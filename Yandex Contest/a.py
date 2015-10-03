
s = 0
for x in range(1, 10000):
	s += len(str(x))
	if s==483:
		print (str(x)+' '+str(s))
		break