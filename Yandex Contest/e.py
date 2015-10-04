
st = "юйбьхуфдтацъьлъюйцшхщуцюхдлщобер"
alph = "йцукенгшщзхъфывапролджэячсмитьбю"

def caesar(s, off):
	alph = "йцукенгшщзхъфывапролджэячсмитьбю"
	result = "";
	for i in range(0, len(s)):
		offset = alph.find(s[i])
		result = result + alph[ (offset + off) % len(alph)]
	print (result+" "+str(off))

for x in range(1, len(alph)):
	caesar(st, x)