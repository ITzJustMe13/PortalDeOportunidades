import 'package:flutter/material.dart';
import 'package:flutter/widgets.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/State/DrawerState.dart';
import 'package:frontend/State/LoginState.dart';

class InMemoryLoginState extends LoginState {
  bool _isLoggedIn = false;

  @override
  bool get isLoggedIn => _isLoggedIn;

  @override
  Future<bool> login(String username, String password, BuildContext context) async {
    // Simula um login bem-sucedido
    _isLoggedIn = true;
    return Future.value(true);
  }

  @override
  Future<void> logout() async {
    // Simula o logout
    _isLoggedIn = false;
    return Future.value();
  }
}

class TestNavigatorObserver extends NavigatorObserver {
  final List<Route<dynamic>> pushedRoutes = [];

  @override
  void didPush(Route<dynamic> route, Route<dynamic>? previousRoute) {
    pushedRoutes.add(route);
    super.didPush(route, previousRoute);
  }
}

void main() {
  late InMemoryLoginState loginState;
  late CustomDrawerState customDrawerState;
  late Widget testWidget;
  late TestNavigatorObserver navigatorObserver;

  setUp(() {
    loginState = InMemoryLoginState();
    customDrawerState = CustomDrawerState(loginState: loginState);

    navigatorObserver = TestNavigatorObserver();

    testWidget = MaterialApp(
      navigatorObservers: [navigatorObserver],
      onGenerateRoute: (settings) {
        return MaterialPageRoute(builder: (context) => Container());
      },
    );
  });

  testWidgets('Navegação para rota válida', (WidgetTester tester) async {
    await tester.pumpWidget(testWidget);

    customDrawerState.navigateToRoute(tester.element(find.byType(Container)), '/home');
    await tester.pumpAndSettle();

    expect(navigatorObserver.pushedRoutes.last.settings.name, '/home');
  });

  testWidgets('Navegação para rota vazia imprime erro no console', (WidgetTester tester) async {
    await tester.pumpWidget(testWidget);

    expect(
      () => customDrawerState.navigateToRoute(tester.element(find.byType(Container)), ''),
      prints(contains('Error: Route cannot be empty.')),
    );
  });

  testWidgets('Usuário logado navega para rota', (WidgetTester tester) async {
    await loginState.login('testUser', 'testPassword', tester.element(find.byType(Container)));
    await tester.pumpWidget(testWidget);

    customDrawerState.ensureUserLoggedIn(tester.element(find.byType(Container)), '/profile');
    await tester.pumpAndSettle();

    expect(navigatorObserver.pushedRoutes.last.settings.name, '/profile');
  });

  testWidgets('Usuário não logado exibe diálogo de login', (WidgetTester tester) async {
    await tester.pumpWidget(testWidget);

    customDrawerState.ensureUserLoggedIn(tester.element(find.byType(Container)), '/profile');
    await tester.pump();

    expect(find.byType(AlertDialog), findsOneWidget);
    expect(find.text('Login necessário'), findsOneWidget);
  });

  testWidgets('Logout limpa estado e navega para rota', (WidgetTester tester) async {
  // Realiza o login simulado
  await loginState.login('testUser', 'testPassword', tester.element(find.byType(Container)));

  await tester.pumpWidget(testWidget);

  // Realiza o logout e navega para a rota de login
  customDrawerState.logout(tester.element(find.byType(Container)), '/login');
  await tester.pumpAndSettle();

  // Verifica se o estado foi limpo e a navegação ocorreu
  expect(loginState.isLoggedIn, isFalse);
  expect(navigatorObserver.pushedRoutes.last.settings.name, '/login');
});

  testWidgets('Diálogo de login navega para tela de login ao clicar no botão de Login', (WidgetTester tester) async {
    await tester.pumpWidget(testWidget);

    customDrawerState.ensureUserLoggedIn(tester.element(find.byType(Container)), '/profile');
    await tester.pump();

    expect(find.byType(AlertDialog), findsOneWidget);

    // Simula o clique no botão "Login"
    await tester.tap(find.text('Login'));
    await tester.pumpAndSettle();

    expect(navigatorObserver.pushedRoutes.last.settings.name, '/login');
  });
}