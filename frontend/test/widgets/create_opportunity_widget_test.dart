import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/State/CreateOpportunityState.dart';
import 'package:frontend/Views/CreateOpportunityScreen.dart';
import 'package:mockito/mockito.dart';
import 'package:provider/provider.dart';

import 'mock_create_opportunity_state.mocks.dart';

void main() {
  late MockCreateOpportunityState mockState;
  late MockBuildContext? mockContext;

  setUp(() {
    mockState = MockCreateOpportunityState();
    mockContext = MockBuildContext();
  });

  testWidgets('Exibe o formulário corretamente', (WidgetTester tester) async {
    // Configura o mock
    when(mockState.isLoading).thenReturn(false);
    when(mockState.errorMessage).thenReturn("");

    // Configura o widget
    await tester.pumpWidget(
      MaterialApp(
        home: ChangeNotifierProvider<CreateOpportunityState>.value(
          value: mockState,
          child: Builder(
            builder: (BuildContext context) {
              // Injeção do mockContext no builder
              mockContext = context as MockBuildContext?;
              return const CreateOpportunityScreen();
            },
          ),
        ),
      ),
    );

    // Verifica se os campos são renderizados
    expect(find.text('Nome da Oportunidade:'), findsOneWidget);
    expect(find.text('Descrição:'), findsOneWidget);
    expect(find.text('Categoria:'), findsOneWidget);
    expect(find.text('Localização:'), findsOneWidget);
    expect(find.text('Data:'), findsOneWidget);
    expect(find.text('Endereço:'), findsOneWidget);
    expect(find.text('Enviar'), findsOneWidget);
  });

  testWidgets('Simula envio bem-sucedido', (WidgetTester tester) async {
    // Configura o mock para simular um estado
    when(mockState.isLoading).thenReturn(false);
    when(mockState.errorMessage).thenReturn("");
    when(mockState.createOpportunity(any, any)).thenAnswer((_) async => Future.value());

    // Configura o widget
    await tester.pumpWidget(
      MaterialApp(
        home: ChangeNotifierProvider<CreateOpportunityState>.value(
          value: mockState,
          child: Builder(
            builder: (BuildContext context) {
              mockContext = context as MockBuildContext?;
              return const CreateOpportunityScreen();
            },
          ),
        ),
      ),
    );

    // Preenche os campos necessários
    await tester.enterText(find.byType(TextFormField).at(0), 'Nova Oportunidade');
    await tester.enterText(find.byType(TextFormField).at(1), 'Descrição da oportunidade');
    await tester.enterText(find.byType(TextFormField).at(2), '10.0');
    await tester.enterText(find.byType(TextFormField).at(3), '5');
    await tester.tap(find.text('Enviar'));

    // Aguarda a próxima renderização
    await tester.pump();

    // Verifica se o método foi chamado
    verify(mockState.createOpportunity(any, any)).called(1);
  });

  testWidgets('Exibe mensagem de erro no envio', (WidgetTester tester) async {
    // Configura o mock para retornar um erro
    when(mockState.isLoading).thenReturn(false);
    when(mockState.errorMessage).thenReturn("Erro ao criar a oportunidade");

    // Configura o widget
    await tester.pumpWidget(
      MaterialApp(
        home: ChangeNotifierProvider<CreateOpportunityState>.value(
          value: mockState,
          child: Builder(
            builder: (BuildContext context) {
              mockContext = context as MockBuildContext?;
              return const CreateOpportunityScreen();
            },
          ),
        ),
      ),
    );

    // Aguarda a próxima renderização
    await tester.pump();

    // Verifica se a mensagem de erro é exibida
    expect(find.text('Erro ao criar a oportunidade'), findsOneWidget);
  });
}