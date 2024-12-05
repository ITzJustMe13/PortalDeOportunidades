
import 'package:frontend/Models/Reservation.dart';

import 'package:flutter_test/flutter_test.dart';
import 'package:mockito/mockito.dart';
import 'package:frontend/State/HistoryReservationState.dart';

import '../mocks.mocks.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  group('HistoryReservationState Tests', () {
    late HistoryReservationState historyReservationState;
    late MockReservationApiHandler mockReservationApiHandler;
    late MockUserApiHandler mockUserApiHandler;
    late MockOpportunityApiHandler mockOpportunityApiHandler;

    setUp(() {
      mockReservationApiHandler = MockReservationApiHandler();
      mockUserApiHandler = MockUserApiHandler();
      mockOpportunityApiHandler = MockOpportunityApiHandler();

      historyReservationState = HistoryReservationState();
      // Injetando as dependências mockadas
      historyReservationState.reservationApiHandler = mockReservationApiHandler;
      historyReservationState.userApiHandler = mockUserApiHandler;
      historyReservationState.opportunityApiHandler = mockOpportunityApiHandler;
    });

    test('Deve retornar erro ao carregar reservas se o userId for -1', () async {
      // Mock user ID retrieval
      when(mockUserApiHandler.getStoredUserID()).thenAnswer((_) async => -1);

      // Testando o método
      await historyReservationState.getReservationList();

      expect(historyReservationState.isLoading, false);
      expect(historyReservationState.error, "Erro ao carregar reservas");
      expect(historyReservationState.reservationList.isEmpty, true);
    });

    test('Deve cancelar a reserva com sucesso', () async {
      final reservationToCancel = Reservation(
        reservationId: 1,
        opportunityId: 101,
        userId: 1,
        reservationDate: DateTime.now(),
        checkInDate: DateTime.now(),
        numOfPeople: 2,
        isActive: true,
        fixedPrice: 100.0,
      );

      // Mock o cancelamento da reserva
      when(mockReservationApiHandler.deleteReservation(1))
          .thenAnswer((_) async => true);

      // Testando o método
      final result = await historyReservationState.cancelReservation(reservationToCancel);

      expect(result, true);
      expect(historyReservationState.isCancelling, false);
      expect(historyReservationState.reservationList.isEmpty, true);
    });

    test('Deve retornar erro ao tentar cancelar uma reserva', () async {
      final reservationToCancel = Reservation(
        reservationId: 1,
        opportunityId: 101,
        userId: 1,
        reservationDate: DateTime.now(),
        checkInDate: DateTime.now(),
        numOfPeople: 2,
        isActive: true,
        fixedPrice: 100.0,
      );

      // Mock falha ao tentar cancelar
      when(mockReservationApiHandler.deleteReservation(1))
          .thenAnswer((_) async => false);

      // Testando o método
      final result = await historyReservationState.cancelReservation(reservationToCancel);

      expect(result, false);
      expect(historyReservationState.isCancelling, false);
      expect(historyReservationState.error, "Erro ao cancelar reserva");
    });
  });
}