
Edit your own config.py file
Enviorment : Python 3.5
Make sure the hdfs is ready to use.

# Usage
- Stream Twitter data and upload to hdfs
- Command: python twitter_stream.py -q <Optional Query>

- Search City Region Tweets
- Command: python twitter_city_search.py -d <Lowercase captial city initial (a,s,b,p,m,h,c)> -q <Optional Query>

- Search User Tweets(Make sure the user list csv is ready)
- Command: python twitter_user_search.py -f <UserID.csv>

# Backup
object_storage .py is used for backup  data from hdfs