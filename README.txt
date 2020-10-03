This is my first file

# initialize your bare repo
$ git init --bare test-repo.git

# clone it and cd to the clone's root directory
$ git clone test-repo.git/ test-clone
Cloning into 'test-clone'...
warning: You appear to have cloned an empty repository.
done.
$ cd test-clone

# make an initial commit in the clone
$ touch README.md
$ git add . 
$ git commit -m "add README"
[master (root-commit) 65aab0e] add README
 1 file changed, 0 insertions(+), 0 deletions(-)
 create mode 100644 README.md

# push to origin (i.e. your bare repo)
$ git push origin master
Counting objects: 3, done.
Writing objects: 100% (3/3), 219 bytes | 0 bytes/s, done.
Total 3 (delta 0), reused 0 (delta 0)
To /Users/jubobs/test-repo.git/
 * [new branch]      master -> master