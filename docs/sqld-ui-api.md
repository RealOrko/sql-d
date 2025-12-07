# 🔌 SqlD Help - SqlD UI API

<div align="right">
	<a href="https://github.com/RealOrko/sql-d/blob/master/docs/_.md#sqld-help---contents">⬆️ [Back to Contents]</a>
</div>

  * 🔍 [Discover API](#discover-api)
  * 📋 [Describe Table API](#describe-table-api)
  * 🔎 [Query Table API](#query-table-api)
  

## 🔍 Discover API

<div align="right">
	<a href="#sqld-help---sqld-ui-api">⬆️ [Back to Top]</a>
</div>
<br/>

You can discover the API. This exposes links which allow you to discover all the shape and content of tables across all the SqlD instances.

*Spec*:

 - `http://{{SqlD.UI.Url}}/api`

*Where*:

 - `{{SqlD.UI.Url}}` = `localhost:5000`

*Example*:

 - http://localhost:5000/api

 💡 *See Also*:

  - [Executing SqlD.UI](https://github.com/RealOrko/sql-d/blob/master/docs/executing.md#sqldui)

![API - Root](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld.ui/api/api-root.png)

## 📋 Describe Table API

<div align="right">
	<a href="#sqld-help---sqld-ui-api">⬆️ [Back to Top]</a>
</div>
<br/>

An example for describing `master`.`newtable1`.

*Spec*:

 - `http://{{SqlD.UI.Url}}/api/query/describe {{TableName}}/{{SqlD.Instance.Url}}`

*Where*:

 - `{{SqlD.UI.Url}}` = `localhost:5000`
 - `{{TableName}}` = `newtable1`
 - `{{SqlD.Instance.Url}}` = `http://localhost:50100/` (URI Encoded)

*Example*:

 - http://localhost:5000/api/query/describe%20newtable1/http%3A%2F%2Flocalhost%3A50100%2F

 💡 *See Also*:

  - [Executing SqlD.UI](https://github.com/RealOrko/sql-d/blob/master/docs/executing.md#sqldui)

![API - Query](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld.ui/api/api-query.png)

## 🔎 Query Table API

<div align="right">
	<a href="#sqld-help---sqld-ui-api">⬆️ [Back to Top]</a>
</div>
<br/>

An example for querying `master`.`newtable1`. 

*Spec*:

 - `http://{{SqlD.UI.Url}}/api/query/select * from {{TableName}}/{{SqlD.Instance.Url}}`

*Where*:

 - `{{SqlD.UI.Url}}` = `localhost:5000`
 - `{{TableName}}` = `newtable1`
 - `{{SqlD.Instance.Url}}` = `http://localhost:50100/` (URI Encoded)

*Example*:

 - http://localhost:5000/api/query/select%20*%20from%20newtable1%20limit%20100/http%3A%2F%2Flocalhost%3A50100%2F

 *See Also*:

  - [Executing SqlD.UI](https://github.com/RealOrko/sql-d/blob/master/docs/executing.md#sqldui)

![API - Query](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld.ui/api/api-query.png)
