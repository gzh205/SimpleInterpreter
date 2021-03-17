# SimpleInterpreter
一个简易的脚本语言解释器
## 一、特点
### 语句的分隔符
与C、Java等语言不同，该脚本多条语句之间的分隔方式更类似于Python。即通过换行符表示语句的结束，并且通过语句前'\t'的个数判断语句的结构。
### 二、变量的数据类型
1.数字
定义一个数字变量i并设置初始值为0
```python
i=0
```
2.字符串
定义一个字符串变量i并设置初始值为"abc"
```python
i="abc"
```
### 三、运算符
#### 1.运算符优先级
|优先级|运算符|
|---|---|---|---|---|
|1|\(|\)|\[|\]|
|2|^|
|3|*|/|%|
|4|\+|\-|
|5|>|<|>=|<=|
|6|==|!=|
|7|&|\||\!|
#### 2.特殊运算符的说明
虽然运算符是仿照C语言设计的，但是这个脚本语言有部分运算符具有该脚本独有的特性  
**(1)+**  
字符串+字符串=字符串(即将两个字符串拼接)  
字符串+数字=字符串；数字+字符串=字符串(将数字转化为字符串并与原字符串拼接)  
**(2)-**  
数字(a)-数字(b)=数字(a-b)
字符串(a)-数字(n)=去掉末尾n位后的字符串a  
数字(n)-字符串(a)=去掉开头n位后的字符串a  
字符串(a)-字符串(b)=去掉字符串a中的出现的字符串b  
**(3)>、<、>=、<=**  
若是两数字进行比较，则有数字a>b为真，则运算符的值为1，反之为0  
字符串则是循环比较每一个字符，对每个字符比较谁的编码值更大(类似于C语言的strcmp)  
**(4)^**  
幂运算，a^b表示a的b次方
### 四、自定义函数
使用关键字function定义一个函数，例如定义一个名为fun1，参数为a,b,c的函数：
```python
function fun1(a,b,c)
	return a
```
其中return a表示该函数的返回值是a
### 五、系统函数
目前系统函数主要有以下5个：
```
Print(str)	将字符串str的内容输出到控制台  
Read()	从控制台读取一个字符串并返回，返回值就是从控制台读取到的字符串  
IsString(str)	检查一个变量是不是字符串，如果是返回1，否则返回0  
ToNumber(str)	将字符串str转化为一个十进制数字  
ReadFile(filename)	打开字符串filename对应的文件并读取为一个字符串，返回读取到的字符串  
WriteFile(filename,input)	将字符串input中的数据写入名字为filename的文件当中(如果文件不存在则会创建一个新文件)
Download(input)	下载网络上的文件(包括html、htm等网页)并通过字符串的方式读取，返回读取到的内容
```
