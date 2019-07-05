# XolidQuery
Dapper XML Query Mapper Plugin! Likes iBatis(MyBatis)


- 1.0.3 Updated : Bugs fix. and property names are ignore case.
- 1.0.2 Updated : isNull, isNullOrEmpty tags added.



## Usage

#### using only Dapper

```C#
using (var conn = Connection)
{
	conn.Query<User>("SELECT id, email, name, age, phone, reg_date AS regDate FROM user WHERE name LIKE CONCAT('%', @name, '%')", new {name = "Smith"});
}
```



#### using XolidQuery with Dapper

##### Repository

*UserRepository.cs*

```C#
using (var conn = Connection)
{
  users = (List<User>) conn.XQuery<User>("User.findAll", user);
}
```



##### Map

*User.xml*

```xml
<?xml version="1.0" encoding="utf-8"?>
<mapper>
    <sql id="selectColumns">
        id,
        email,
        name,
        age,        
        phone,
        reg_date AS regDate
    </sql>
    
    <select id="findAll">
        SELECT
            <include refid="selectColumns" />
        FROM user
        <where>
            <isNotNull property="name">
                name LIKE CONCAT('%', @name, '%')
            </isNotNull>
            <isNotNull property="email">
                AND email LIKE CONCAT('%', @email, '%')
            </isNotNull>
            <isNotNull property="age">
                AND age = @age
            </isNotNull>
            <isNotNull property="phone">
                AND phone LIKE CONCAT('%', @phone, '%')
            </isNotNull>
        </where>
        ORDER BY id DESC
    </select>
</mapper>
```



## Supported Tag List (Likes MyBatis)

- select, insert, update, delete
- isNotNull, isNull `1.0.2`, isNullOrEmpty `1.0.2`
- if
- choose, when, otherwise
- include

