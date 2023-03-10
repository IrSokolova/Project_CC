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

// ???
//Identifier
//    : 
//    ;
    
Declaration
    : VariableDeclaration                           { $$ = new Declaration($1, NULL, NULL); }
    | TypeDeclaration				    { $$ = new Declaration(NULL, $1, NULL); }
    | RoutineDeclaration                            { $$ = new Declaration(NULL, NULL, $1); }
    ;

// what does get_yyextra_string() mean?
// and need to decide about IDENTIFIER
VariableDeclaration
    : VAR IDENTIFIER COLON Type IS InitialValue     { $$ = new VariableDeclaration(get_yyextra_string(), $4, $6); }
    | VAR IDENTIFIER COLON Type                     { $$ = new VariableDeclaration(get_yyextra_string(), $4, NULL); }
    ;
 
VariableDeclarations
    : 					            { $$ = new VariableDeclarations(NULL, NULL); }
    | VariableDeclaration VariableDeclarations      { $$ = new VariableDeclarations($1, $2); }
    ;

// how it should look like?
TypeDeclaration
    : 					            { $$ = new TypeDeclaration(NULL, NULL); }
    ;
     
RoutineDeclaration
    : ROUTINE IDENTIFIER PARENTHESES_L Parameters PARENTHESES_R RoutineReturnType RoutineInsights { $$ = new RoutineDeclaration(get_yyextra_string(), $4, $6, $7); }
    ;

Parameters
    :                         			     { $$ = NULL; }
    |  ParameterDeclaration ParameterDeclarations    { $$ = new Parameters($1, $2); }
    ;


ParameterDeclaration
    : IDENTIFIER COLON Type                          { $$ = new ParameterDeclaration(get_yyextra_string(), $3); }
    ;

// ====================	================================================================================================


ParameterDeclarations
    :                                                { $$ = NULL; }
    | COMMA ParameterDeclaration ParameterDeclarations    { $$ = new ParametersDeclaration($2, $3); }
    ;


          
Action
    : 						   { $$ = NULL; root = $$; }
    | Declaration Action                           { $$ = new Action($1, $2); root = $$; }
    | DECLARATION_SEPARATOR Action                 { $$ = $2; root = $$; }                                       
    ;


Actions
    : 						   { $$ = new VariableDeclarations(NULL, NULL); }
    | Action Actions                 		   { $$ = new Actions($1, $2); }
    ;
	
	
	
	
