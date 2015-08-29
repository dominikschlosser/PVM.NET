# PVM.NET
Process Virtual Machine

[![Build Status](https://travis-ci.org/dkschlos/PVM.NET.svg?branch=master)](https://travis-ci.org/dkschlos/PVM.NET)
<a href="https://scan.coverity.com/projects/dkschlos-pvm-net">
  <img alt="Coverity Scan Build Status"
       src="https://scan.coverity.com/projects/6186/badge.svg"/>
</a>

Based on concept described here: http://docs.jboss.com/jbpm/pvm/article/

Initial Goals:
 - Fully type safe data context 
 - Easy in-language use (Fluent API)
 - Transactions "done right"
 - Strong user/role concept
 - Built-in possibility to skip parts of the process or change flow on demand (see below)
 - Testable
 - Extensible

Long-term goals:
 - DSL for easier usage
 - GUI-Integration
 - Validation
 

This project is mainly an experiment with the goal to create a process engine that is tailored to "real world"-needs.
The idea came from a failed project using one of the many "classical" workflow-engines.
The main problem im trying to solve here was, that real world workflows are not workflows by definition:
Decisions can be withdrawn, workflows might never come to an end and our stakeholders would have liked to have the possibility to react on unforeseen circumstances by skipping parts or even changing the flow on demand.

