from os import listdir
from os.path import isfile
from os.path import join as joinpath
import hashlib
import os

myPath = "tetris-tests-2015\\"
myResult = "results\\"
myProg = "Tetris.exe"

def md5(fname):
    hash = hashlib.md5()
    with open(fname, "rb") as f:
        for chunk in iter(lambda: f.read(4096), b""):
            hash.update(chunk)
    return hash.hexdigest()

myjson = filter(lambda x: x.endswith('.json'), listdir(myPath))

Ok = 0
Fail = 0
for i in myjson:
    if isfile(joinpath(myPath,i)):
    	BaseFile = str(i)[:-5]    	
    	md5Hash = md5(myPath + BaseFile+".txt")

    	print ("Check {0}".format(i))    	
    	print ("MD5 correct: {0}".format(md5Hash))
    	result = myResult + BaseFile
    	cmd = "{0} {1} > {2}".format(myProg, joinpath(myPath, i), result)
    	os.system(cmd)
    	md5Another = md5(result)
    	print ("MD5 my answer: {0}".format(md5Another))
    	if md5Hash==md5Another:
    		print ("Test {0} Passed! Ok!".format(BaseFile))
    		Ok+=1
    	else:
    		print ("Test {0} Failed! Wrong!".format(BaseFile))
    		Fail+=1
    	print ("###########################")

print ("Passed {0} tests.\r\n Failed {1} tests.".format(Ok, Fail))