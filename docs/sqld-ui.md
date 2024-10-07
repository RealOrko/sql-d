# SqlD Help - SqlD UI

<div align="right">
	<a href="https://github.com/RealOrko/sql-d/blob/master/docs/_.md#sqld-help---contents">[Back to Contents]</a>
</div>

  * [Registered Servers](#registered-servers)
  * [Discovering Tables](#discovering-tables)
  * [Creating a Table](#creating-a-table)
  * [Inserting Rows](#inserting-rows)
  * [Querying a Table](#querying-a-table)
  * [Dropping a Table](#dropping-a-table)

## Registered Servers

<div align="right">
	<a href="#sqld-help---sqld-ui">[Back to Top]</a>
</div>
<br/>

When you first browse to an instance of SqlD, you will be greet by the home page. You can use the little blue button's to connect to 
each individual instance of SqlD. The default configuration has 5 instances running. 

 - `registry`(http://localhost:50095/) all services are registered here on start up
 - `master`(http://localhost:50100/) replicates to `slave 1`, `slave 2` and `slave 3` 
 - `slave 1`(http://localhost:50101/)
 - `slave 2`(http://localhost:50102/)
 - `slave 3`(http://localhost:50103/)

![Query](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld.ui/home-page.png)

 *See Also*:

  - [About](https://github.com/RealOrko/sql-d/blob/master/docs/about.md)
  - [Prerequisites](https://github.com/RealOrko/sql-d/blob/master/docs/prerequisites.md)
  - [Executing SqlD.UI](https://github.com/RealOrko/sql-d/blob/master/docs/executing.md#executing-sqldui)

## Discovering Tables

<div align="right">
	<a href="#sqld-help---sqld-ui">[Back to Top]</a>
</div>
<br/>

You can query `sqlite_master` to find out which tables have been created on currently connected SqlD instance. From the results we can see 
`master` (http://localhost:50100) has 2 tables, `newtable1` and `sqlite_sequence`.

![Query - Master Query](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld.ui/home-page-query.png)

 *See Also*:

  - [About](https://github.com/RealOrko/sql-d/blob/master/docs/about.md)
  - [Prerequisites](https://github.com/RealOrko/sql-d/blob/master/docs/prerequisites.md)
  - [Executing SqlD.UI](https://github.com/RealOrko/sql-d/blob/master/docs/executing.md#executing-sqldui)

## Creating a Table

<div align="right">
	<a href="#sqld-help---sqld-ui">[Back to Top]</a>
</div>
<br/>

The example below shows you how to create a table called `newtable1` with columns for `id`, `name` and `created`. `id` is a sequence that auto increments, 
`name` can hold text and `created` is a datetime that defaults to a current timestamp.

![Query - Create Table](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld.ui/home-page-create-table.png)

 *See Also*:

  - [About](https://github.com/RealOrko/sql-d/blob/master/docs/about.md)
  - [Prerequisites](https://github.com/RealOrko/sql-d/blob/master/docs/prerequisites.md)
  - [Executing SqlD.UI](https://github.com/RealOrko/sql-d/blob/master/docs/executing.md#executing-sqldui)

## Inserting Rows

<div align="right">
	<a href="#sqld-help---sqld-ui">[Back to Top]</a>
</div>
<br/>

The example below shows you how to insert 5 rows into `newtable1`. 

![Query - Insert Rows](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld.ui/home-page-insert-rows.png)

 *See Also*:

  - [About](https://github.com/RealOrko/sql-d/blob/master/docs/about.md)
  - [Prerequisites](https://github.com/RealOrko/sql-d/blob/master/docs/prerequisites.md)
  - [Executing SqlD.UI](https://github.com/RealOrko/sql-d/blob/master/docs/executing.md#executing-sqldui)

## Querying a Table

<div align="right">
	<a href="#sqld-help---sqld-ui">[Back to Top]</a>
</div>
<br/>

The example below shows you how to query `newtable1`. 

![Query - Query Table](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld.ui/home-page-query-newtable.png)

 *See Also*:

  - [About](https://github.com/RealOrko/sql-d/blob/master/docs/about.md)
  - [Prerequisites](https://github.com/RealOrko/sql-d/blob/master/docs/prerequisites.md)
  - [Executing SqlD.UI](https://github.com/RealOrko/sql-d/blob/master/docs/executing.md#executing-sqldui)

## Dropping a Table

<div align="right">
	<a href="#sqld-help---sqld-ui">[Back to Top]</a>
</div>
<br/>

The example below shows you how to drop `newtable1`. 

![Query - Drop Table](https://github.com/RealOrko/sql-d/blob/master/docs/images/sqld.ui/home-page-drop-table.png)

 *See Also*:

  - [About](https://github.com/RealOrko/sql-d/blob/master/docs/about.md)
  - [Prerequisites](https://github.com/RealOrko/sql-d/blob/master/docs/prerequisites.md)
  - [Executing SqlD.UI](https://github.com/RealOrko/sql-d/blob/master/docs/executing.md#executing-sqldui)

