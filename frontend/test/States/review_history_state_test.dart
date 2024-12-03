import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/Enums/Gender.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/OpportunityImg.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Models/Review.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/State/ReviewHistoryState.dart';
import 'package:mockito/mockito.dart';
import '../mocks.mocks.dart';

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  late ReviewHistoryState reviewHistoryState;
  late MockUserApiHandler mockUserApiHandler;
  late MockReservationApiHandler mockReservationApiHandler;
  late MockOpportunityApiHandler mockOpportunityApiHandler;
  late MockReviewApiHandler mockReviewApiHandler;

  setUp(() {
    mockUserApiHandler = MockUserApiHandler();
    mockReservationApiHandler = MockReservationApiHandler();
    mockOpportunityApiHandler = MockOpportunityApiHandler();
    mockReviewApiHandler = MockReviewApiHandler();

    reviewHistoryState = ReviewHistoryState()
      ..userApiHandler = mockUserApiHandler
      ..reservationApiHandler = mockReservationApiHandler
      ..opportunityApiHandler = mockOpportunityApiHandler
      ..reviewApiHandler = mockReviewApiHandler;
  });

  test('Deve carregar a lista de comentários com sucesso', () async {
    final reservation = Reservation(
      reservationId: 1,
      opportunityId: 101,
      userId: 1,
      reservationDate: DateTime(2024, 1, 10),
      checkInDate: DateTime(2024, 2, 10),
      numOfPeople: 4,
      isActive: true,
      fixedPrice: 100.0,
    );

    final opportunityImg = OpportunityImg(
      imgId: 1,
      opportunityId: 101,
      imageBase64: 'base64EncodedImageString',
    );

    final opportunity = Opportunity(
      opportunityId: 101,
      name: 'Opportunity 101',
      price: 50.0,
      vacancies: 10,
      isActive: true,
      category: OppCategory.AGRICULTURA,
      description: 'Description for Opportunity 101',
      location: Location.ACORES,
      address: '123 Main St, New York, NY',
      userId: 1,
      reviewScore: 4.5,
      date: DateTime(2024, 2, 10),
      isImpulsed: false,
      opportunityImgs: [opportunityImg],
    );

    final review = Review(
      reservationId: 1,
      rating: 5,
      description: 'Great experience!',
    );

    when(mockUserApiHandler.getStoredUserID()).thenAnswer((_) async => 1);
    when(mockReservationApiHandler.getReservationById(1)).thenAnswer((_) async => reservation);
    when(mockOpportunityApiHandler.getOpportunityByID(101)).thenAnswer((_) async => opportunity);
    when(mockReviewApiHandler.getReviewsByUserId(1)).thenAnswer((_) async => [review]);

    await reviewHistoryState.getReviewList();

    expect(reviewHistoryState.isLoading, false);
    expect(reviewHistoryState.error, '');
    expect(reviewHistoryState.reviewList.length, 1);
    expect(reviewHistoryState.reviewList.first.review.description, 'Great experience!');
    expect(reviewHistoryState.reviewList.first.opportunity.opportunityImgs.length, 1);
    expect(reviewHistoryState.reviewList.first.opportunity.opportunityImgs.first.imageBase64, 'base64EncodedImageString');
  });

  test('Deve lidar com erro ao carregar comentários', () async {
    when(mockUserApiHandler.getStoredUserID()).thenAnswer((_) async => -1);

    await reviewHistoryState.getReviewList();

    expect(reviewHistoryState.isLoading, false);
    expect(reviewHistoryState.error, 'Erro ao carregar comentários');
    expect(reviewHistoryState.reviewList.isEmpty, true);
  });
}