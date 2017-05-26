
*Max Booth and Emily Johnson*


#Week 0


##GIT setup

**Pulling from a Repository**
After receiving local admin priviledges and downloading the GIT extensions, you can begin 
setting up your connection to the bitbucket repository that dimensional projects are stored in
at stash.dimensional.com.  In order to connect to the server, you will need to be given access
by someone who is already an administrator for the server.  After connecting, you will need to 
pull the git repository to your own computer.  To do this, open GIT extensions and generate a
private and public key by goign to tools->puTTy->generate or import key.  Then click generate
to create a new pair of private and public keys.  Be sure to save the private key on both your
local machine and your network drive, since if you lose your private key you will have to 
repeat this whole process (while a public key you can always generate from a private key.)  
Now go to your user settings on stash.dimensional.com and add the generated public key to your 
key configuration settings.  Now, go to the repository that you would like to clone and click 
the "clone" button.  This will bring up the ssh address of the repository that you will use to
clone the repository.  Copy this address and return to GIT extensions, and click "clone
 repository" on the side.  Paste the ssh address that was copied into the "repository to clone" 
field and choose a destination folder to store the desired repository.  Load the PRIVATE SSH 
key that you saved from puTTy (using the button at the bottom of the popup) and clone.


**Pulling to a Repository**
After you have pulled a clone of the repository to your computer, you will need to be able
to push your own changes onto the dimensional server's repository.  To do this, you will need
to have write permission to the server (given to you by an administrator.)  You can use either
GIT extensiosn or the command line to do this.  Since the pluralsight GIT videos use the command
line, using the command line to do this is probably easier (but if you decide to try GIT 
extensions, it can be done by using the blue push button at the top the window.)  To do so, you
will need to set up your ssh configuration, which can be done by following the directions of the
following website:

https://serverfault.com/questions/194567/how-do-i-tell-git-for-windows-where-to-find-my-private-rsa-key

You will need to open puTTy in GIT extensions and convert the private key that you generated as
an openSSH key and save is as id_rsa a folder named ".ssh" in your home directory (you can find
 your home directory by finding what variable HOME is pointing to.  This can be viewed by typing
*env* in cmd.)  If you do not have a HOME variable, then you can set it by using setx and
 choosing the directory you would like to use for home (such as c:\users\yourprofile or U:\.) After 
this, you will be able to be able to use ssh to push to bitbucket. 
In order to actually do this push, you will have to do the following commands (as per the git viedos):
git branch *branchname* 		create branch
git checkout *branchname*		switch to that branch
*do some changes*
git add -a				adds all the changes youve made
git commit -m "message"			create a commit to your branch
git push				pushes to the dimensional repository

*Note:  you can use git branch to tell what branch you are on, and git status to find out what is 
being loaded into the commit.*

If your branch isn't already pointing to stash.dimensional, git will give you an error message
telling you to use --set-upstream to set it to the origin (which is good, just copy that line.)
Now you should have pushed your branch to dimensional server!


##SQL setup


http://itproguru.com/expert/2014/09/how-to-fix-login-failed-for-user-microsoft-sql-server-error-18456-step-by-step-add-sql-administrator-to-sql-management-studio/

https://stackoverflow.com/questions/20923015/login-to-microsoft-sql-server-error-18456

https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/connect-to-sql-server-when-system-administrators-are-locked-out



##Tiny Returns Project

We look at the computed data through the following entry points: Web Report
and Excel Report


###Web Report

Through Visual Studio, right click on External.Web to Set as StartUp Project. The website is 
implemented through Web.Controllers.PortfolioPerformanceController and called with Index().
We need to GetPortfolioPerformance() for each of the portfolios we want to display.
We CreatePortfolioModel(PortfolioWithPerformance, MonthYear) where the PortfolioWithPerformance 
is an entry in the TinyReturns webpage chart. Each entry has a number, name, and ReturnSeries 
and a benchmark array where each benchmark has a number, name, and ReturnSeries
where ReturnSeries: an array of monthly returns; financial math
An entry is created by:
PublicWebReportFacade creates an entry (Number, Name, OneMonth,ThreeMonth, YearToDate)