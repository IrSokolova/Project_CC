// TOKENS

%token Undefined
%token Keywords
%token BooleanLiterals
%token Operators
%token OpenPunctuators
%token ClosePunctuators
%token Identifiers
%token FloatingLiterals
%token IntegerLiteral

%type <string> Identifiers ClosePunctuators OpenPunctuators Operators Keywords Undefined
%type  <float_value> IntegerLiteral FloatingLiterals

%type <Identifier> Identifier
%type <Declaration> Declaration
%type <VariableDeclaration> VariableDeclaration
%type <VariableDeclarations> VariableDeclarations
%type <TypeDeclaration> TypeDeclaration
%type <RoutineDeclaration> RoutineDeclaration
%type <Parameters> Parameters
%type <ParameterDeclaration> ParameterDeclaration
%type <ParameterDeclarations> ParameterDeclarations
%type <Value> Value
%type <Expression> Expression
%type <Expressions> Expressions
%type <Relation> Relation
%type <Operation> Operation
%type <Operand> Operand
%type <Comparison> Comparison
%type <Single> Single
%type <Modifiable> Modifiable
%type <Operator> Operator
%type <ComparisonOperator> ComparisonOperator
%type <MathematicalOperator> MathematicalOperator
%type <LogicalOperator> LogicalOperator
%type <MultipleRelation> MultipleRelation
%type <Type> Type
%type <PrimitiveType> PrimitiveType
%type <ArrayType> ArrayType
%type <RecordType> RecordType
%type <Action> Action
%type <Actions> Actions
%type <RoutineReturnType> RoutineReturnType
%type <RoutineInsights> RoutineInsights
%type <Return> Return
%type <Body> Body
%type <Statement> Statement
%type <Assignment> Assignment
%type <RoutineCall> RoutineCall
%type <WhileLoop> WhileLoop
%type <ForLoop> ForLoop
%type <Range> Range
%type <IfStatemen> IfStatement


%start Action

// RULES
%%

Action
    : Declaration Action                           { $$ = new Program($1, $2); root = $$; }
    | DECLARATION_SEPARATOR Action                 { $$ = $2; root = $$; }
    |                                               { $$ = NULL; root = $$; }
    ;

Actions
    : /* empty */
    | Action Actions                 { $$ = $2; root = $$; }
    |                                               { $$ = NULL; root = $$; }
    ;
	
Declaration
    : SimpleDeclaration                             { $$ = new Declaration($1, NULL); }
    | RoutineDeclaration                            { $$ = new Declaration(NULL, $1); }
    ;

SimpleDeclaration
    : VariableDeclaration                           { $$ = new SimpleDeclaration($1, NULL); }
    | TypeDeclaration                               { $$ = new SimpleDeclaration(NULL, $1); }
    ;

VariableDeclaration
    : VAR IDENTIFIER COLON Type InitialValue        { $$ = new VariableDeclaration(get_yyextra_string(), $4, $5, NULL); }
    | VAR IDENTIFIER IS Expression                  { $$ = new VariableDeclaration(get_yyextra_string(), NULL, NULL, $4); }
    ;

InitialValue
    : IS Expression                                 { $$ = new InitialValue($2); }
    |                                               { $$ = NULL; }
    ;

TypeDeclaration
    : TYPE IDENTIFIER IS Type                       { $$ = new TypeDeclaration(get_yyextra_string(), $4); }
    ;

Type
    : PrimitiveType                                 { $$ = new Type($1, NULL, NULL, ""); }
    | ArrayType                                     { $$ = new Type(NULL, $1, NULL, ""); }
    | RecordType                                    { $$ = new Type(NULL, NULL, $1, ""); }
    | IDENTIFIER                                    { $$ = new Type(NULL, NULL, NULL, get_yyextra_string()); }
    ;

PrimitiveType
    : INTEGER                                       { $$ = new PrimitiveType(true, false, false); }
    | REAL                                          { $$ = new PrimitiveType(false, true, false); }
    | BOOLEAN                                       { $$ = new PrimitiveType(false, false, true); }
    ;

ArrayType
    : ARRAY BRACKETS_L Expression BRACKETS_R Type   { $$ = new ArrayType($3, $5); }
    | ARRAY BRACKETS_L BRACKETS_R Type              { $$ = new ArrayType(NULL, $4); }
    ;

RecordType
    : RECORD VariableDeclarations END               { $$ = new RecordType($2); }
    ;

VariableDeclarations
    : VariableDeclaration VariableDeclarations      { $$ = new VariableDeclarations($1, $2); }
    |                                               { $$ = NULL; }
    ;

RoutineDeclaration
    : ROUTINE IDENTIFIER PARENTHESES_L Parameters PARENTHESES_R TypeInRoutineDeclaration BodyInRoutineDeclaration { $$ = new RoutineDeclaration(get_yyextra_string(), $4, $6, $7); }
    | ROUTINE IDENTIFIER PARENTHESES_L Parameters PARENTHESES_R BodyInRoutineDeclaration  { $$ = new RoutineDeclaration(get_yyextra_string(), $4, NULL, $6); }
    ;

ReturnInRoutine
    : RETURN Expression                                                          { $$ = new ReturnInRoutine($2); }
    |                                                                            { $$ = NULL; }
    ;

Parameters
    :  ParameterDeclaration ParametersDeclaration     { $$ = new Parameters($1, $2); }
    |                                                 { $$ = NULL; }
    ;

ParameterDeclaration
    : IDENTIFIER COLON Type                                   { $$ = new ParameterDeclaration(get_yyextra_string(), $3); }
    ;

ParametersDeclaration
    : COMMA ParameterDeclaration ParametersDeclaration        { $$ = new ParametersDeclaration($2, $3); }
    |                                                         { $$ = NULL; }
    ;

TypeInRoutineDeclaration
    : COLON Type                                      { $$ = new TypeInRoutineDeclaration($2); }
    ;

BodyInRoutineDeclaration
    : IS Body ReturnInRoutine END                                   { $$ = new BodyInRoutineDeclaration($2, $3); }
    |                                                               { $$ = NULL; }
    ;

Body
    : SimpleDeclaration Body                        { $$ = new Body($1, NULL, $2); }
    | Statement Body                                { $$ = new Body(NULL, $1, $2); }
    |                                               { $$ = NULL; }
    ;

Statement
    : Assignment                                    { $$ = new Statement($1, NULL, NULL, NULL, NULL); }
    | RoutineCall                                   { $$ = new Statement(NULL, $1, NULL, NULL, NULL); }
    | WhileLoop                                     { $$ = new Statement(NULL, NULL, $1, NULL, NULL); }
    | ForLoop                                       { $$ = new Statement(NULL, NULL, NULL, $1, NULL); }
    | IfStatement                                   { $$ = new Statement(NULL, NULL, NULL, NULL, $1); }
    ;

Assignment
    : ModifiablePrimary ASSIGN Expression             { $$ = new Assignment($1, $3); }
    ;

RoutineCall
    : IDENTIFIER ExpressionInRoutineCall            { $$ = new RoutineCall(get_yyextra_string(), $2); }
    ;

ExpressionInRoutineCall
    : PARENTHESES_L Expression ExpressionsInRoutineCall PARENTHESES_R   { $$ = new ExpressionInRoutineCall($2, $3); }
    |                                                                   { $$ = NULL; }
    ;

ExpressionsInRoutineCall
    : COMMA Expression ExpressionsInRoutineCall        { $$ = new ExpressionsInRoutineCall($2, $3); }
    |                                                 { $$ = NULL; }
    ;

WhileLoop
    : WHILE Expression LOOP Body END                { $$ = new WhileLoop($2, $4); }
    ;

ForLoop
    : FOR IDENTIFIER IN Reverse Range LOOP Body END { $$ = new ForLoop(get_yyextra_string(), $4, $5, $7); }
    ;

Range
    : Expression RANGER Expression                      { $$ = new Range($1, $3); }
    ;

Reverse
    : REVERSE                                       { $$ = new Reverse(true); }
    |                                               { $$ = new Reverse(false); }
    ;

IfStatement
    : IF Expression THEN Body ElseInIfStatement END { $$ = new IfStatement($2, $4, $5); }
    ;

ElseInIfStatement
    : ELSE Body                                     { $$ = new ElseInIfStatement($2); }
    |                                               { $$ = NULL; }
    ;

Expression
    : Relation MultipleRelationsInExpression        { $$ = new Expression($1, $2); }
    ;

MultipleRelationsInExpression
    : LogicalOperator Relation MultipleRelationsInExpression    { $$ = new MultipleRelationsInExpression($1, $2, $3); }
    |                                                           { $$ = NULL; }
    ;

LogicalOperator
    : AND                                           { $$ = new LogicalOperator(Lexems::Operators::AND); }
    | OR                                            { $$ = new LogicalOperator(Lexems::Operators::OR); }
    | XOR                                           { $$ = new LogicalOperator(Lexems::Operators::XOR); }
    ;

Relation
    : Simple ComparisonInRelation                   { $$ = new Relation($1, $2); }
    ;

ComparisonInRelation
    : ComparisonOperator Simple                     { $$ = new ComparisonInRelation($1, $2); }
    |                                               { $$ = NULL; }
    ;

ComparisonOperator
    : LESS                                           { $$ = new ComparisonOperator(Lexems::Operators::LESS); }
    | LESS_EQ                                        { $$ = new ComparisonOperator(Lexems::Operators::LESS_EQ); }
    | GREATER                                        { $$ = new ComparisonOperator(Lexems::Operators::GREATER); }
    | GREATER_EQ                                     { $$ = new ComparisonOperator(Lexems::Operators::GREATER_EQ); }
    | EQ                                             { $$ = new ComparisonOperator(Lexems::Operators::EQ); }
    | NOT_EQ                                         { $$ = new ComparisonOperator(Lexems::Operators::NOT_EQ); }
    ;

Simple
    : Factor Factors                                { $$ = new Simple($1, $2); }
    ;

Factors
    : SimpleOperator Factor Factors                 { $$ = new Factors($1, $2, $3); }
    |                                               { $$ = NULL; }
    ;

SimpleOperator
    : MULT                                           { $$ = new SimpleOperator(Lexems::Operators::MULT); }
    | DIV                                            { $$ = new SimpleOperator(Lexems::Operators::DIV); }
    | REMAINDER                                      { $$ = new SimpleOperator(Lexems::Operators::REMAINDER); }
    ;

Factor
    : Summand Summands                               { $$ = new Factor($1, $2); }
    ;

Summands
    : Sign Summand Summands                         { $$ = new Summands($1, $2, $3); }
    |                                               { $$ = NULL; }
    ;

Summand
    : Primary                                       { $$ = new Summand($1, NULL); }
    | PARENTHESES_L Expression PARENTHESES_R        { $$ = new Summand(NULL, $2); }
    ;


Primary
    : INTEGER_LITERAL                               { $$ = new Primary(Lexems::Keywords::INTEGER, (float)get_yyextra_int(), false, NULL,  NULL); }
    | Sign INTEGER_LITERAL                          { $$ = new Primary(Lexems::Keywords::INTEGER, (float)get_yyextra_int(), false, $1, NULL); }
    | NOT INTEGER_LITERAL                           { $$ = new Primary(Lexems::Keywords::INTEGER, (float)get_yyextra_int(), true, NULL, NULL); }
    | REAL_LITERAL                                  { $$ = new Primary(Lexems::Keywords::REAL, get_yyextra_float(), false, NULL, NULL); }
    | Sign REAL_LITERAL                             { $$ = new Primary(Lexems::Keywords::REAL, get_yyextra_float(), false, $1, NULL); }
    | TRUE                                          { $$ = new Primary(Lexems::Keywords::BOOLEAN, (float)true, false, NULL, NULL); }
    | FALSE                                         { $$ = new Primary(Lexems::Keywords::BOOLEAN, (float)false, false, NULL, NULL); }
    | ModifiablePrimary                             { $$ = new Primary("", 0, false, NULL, $1); }
    ;

Sign
    : PLUS                                           { $$ = new Sign(Lexems::Operators::PLUS); }
    | MINUS                                          { $$ = new Sign(Lexems::Operators::MINUS); }
    ;

ModifiablePrimary
    : IDENTIFIER Identifiers                         { $$ = new ModifiablePrimary(get_yyextra_string(), $2); }
    ;

Identifiers
    : DOT IDENTIFIER Identifiers                     { $$ = new Identifiers(get_yyextra_string(), NULL, $3); }
    | BRACKETS_L Expression BRACKETS_R Identifiers   { $$ = new Identifiers("", $2, $4); }
    |                                                { $$ = NULL; }
    ;

%%
