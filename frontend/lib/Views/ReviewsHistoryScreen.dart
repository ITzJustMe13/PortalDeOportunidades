import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Enums/Gender.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:frontend/Models/Review.dart';
import '../Models/User.dart';

class ReviewsHistoryScreen extends StatefulWidget {
  const ReviewsHistoryScreen({super.key});

  @override
  _ReviewsHistoryScreenState createState() => _ReviewsHistoryScreenState();
}

class _ReviewsHistoryScreenState extends State<ReviewsHistoryScreen> {
  User user = User(
      userId: 1,
      firstName: "Antonio",
      lastName: "Silva",
      email: "antonio.silva@gmail.com",
      password: "123456789",
      birthDate: DateTime(2004),
      registrationDate: DateTime.now(),
      cellPhoneNumber: 911232938,
      gender: Gender.MASCULINO,
      image: "teste");

  List<Review> reviews = List.generate(
    50, // Número de reviews para teste
    (index) => Review(
      reservationId: index + 1,
      rating: (index % 5) + 1, // Avaliação de 1 a 5
      description: "Review ${index + 1}: This is a sample review.",
    ),
  );

  int currentPage = 1; // Página atual
  static const int itemsPerPage = 16; // Número máximo de cards por página

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1200) {
            return _buildTabletLayout();
          } else {
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout() {
    return _buildReviewsList();
  }

  Widget _buildTabletLayout() {
    return _buildReviewsList();
  }

  Widget _buildDesktopLayout() {
    return _buildReviewsList();
  }

  Widget _buildReviewsList() {
    // Determina os índices dos reviews exibidos na página atual
    int startIndex = (currentPage - 1) * itemsPerPage;
    int endIndex = (startIndex + itemsPerPage) > reviews.length
        ? reviews.length
        : (startIndex + itemsPerPage);

    // Obtém os reviews da página atual
    List<Review> paginatedReviews = reviews.sublist(startIndex, endIndex);

    return Column(
      children: [
        Expanded(
          child: ListView.builder(
            padding: const EdgeInsets.all(16.0),
            itemCount: paginatedReviews.length,
            itemBuilder: (context, index) {
              return _buildReviewCard(paginatedReviews[index]);
            },
          ),
        ),
        _buildPaginationControls(),
      ],
    );
  }

  Widget _buildPaginationControls() {
    int totalPages = (reviews.length / itemsPerPage).ceil();

    return Padding(
      padding: const EdgeInsets.all(8.0),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          IconButton(
            icon: const Icon(Icons.arrow_back),
            onPressed: currentPage > 1
                ? () {
                    setState(() {
                      currentPage--;
                    });
                  }
                : null, // Desabilita se estiver na primeira página
          ),
          Text(
            'Page $currentPage of $totalPages',
            style: const TextStyle(fontSize: 16),
          ),
          IconButton(
            icon: const Icon(Icons.arrow_forward),
            onPressed: currentPage < totalPages
                ? () {
                    setState(() {
                      currentPage++;
                    });
                  }
                : null, // Desabilita se estiver na última página
          ),
        ],
      ),
    );
  }

  Widget _buildReviewCard(Review review) {
    Reservation reservation = _getReservation(review.reservationId);
    Opportunity opportunity = _getOpportunity(reservation.opportunityId);

    return Card(
      margin: const EdgeInsets.only(bottom: 16.0),
      elevation: 4,
      shape: RoundedRectangleBorder(
        borderRadius: BorderRadius.circular(12.0),
      ),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            _buildOpportunityName(opportunity),
            const SizedBox(height: 8),
            _buildReservationDate(reservation),
            const SizedBox(height: 8),
            _buildRating(review),
            const SizedBox(height: 8),
            _buildDescription(review),
          ],
        ),
      ),
    );
  }

  Widget _buildOpportunityName(Opportunity opportunity) {
    return Text(
      opportunity.name,
      style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
    );
  }

  Widget _buildReservationDate(Reservation reservation) {
    return Text(
      "Reservation Date: ${reservation.reservationDate}",
      style: const TextStyle(fontSize: 14, color: Colors.grey),
    );
  }

  Widget _buildRating(Review review) {
    return Row(
      children: [
        const Text(
          "Rating: ",
          style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
        ),
        Text(
          "${review.rating}/5",
          style: const TextStyle(fontSize: 16),
        ),
      ],
    );
  }

  Widget _buildDescription(Review review) {
    return Text(
      review.description ??
          "No description available", // Se description for null, exibe o texto padrão.
      style: const TextStyle(fontSize: 14),
    );
  }

  Reservation _getReservation(int reservationId) {
    return Reservation(
        opportunityId: reservationId,
        userId: 1,
        checkInDate: DateTime(2024, 12, 1),
        reservationDate: DateTime.now(),
        numOfPeople: 2,
        isActive: true,
        fixedPrice: 12.20);
  }

  Opportunity _getOpportunity(int opportunityId) {
    return Opportunity(
        name: "Experience $opportunityId",
        price: 12.20,
        vacancies: 4,
        isActive: true,
        category: OppCategory.ARTESANATO,
        description: "Unique experience $opportunityId",
        location: Location.LISBOA,
        address: "Rua Example, Lisboa",
        userId: 1,
        reviewScore: 4.1,
        date: DateTime(2024, 12, 1),
        isImpulsed: false,
        opportunityImgs: []);
  }
}
