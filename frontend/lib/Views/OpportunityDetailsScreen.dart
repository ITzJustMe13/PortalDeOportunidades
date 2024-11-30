import 'package:flutter/material.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:frontend/Services/payment_api_handler.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Components/OpportunityImages.dart';
import 'package:frontend/Components/OpportunityLocationMap.dart';
import 'package:frontend/Components/OpportunityDetails.dart';
import 'package:frontend/Components/OpportunityAdditionalInfo.dart';
import 'package:frontend/Components/ReservationButton.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/Reservation.dart';
import 'package:intl/intl.dart';
import 'package:frontend/Services/payment_service.dart';
import 'package:url_launcher/url_launcher.dart';

class OpportunityDetailsScreen extends StatefulWidget {
  final bool isReservation;
  final Opportunity opportunity;

  const OpportunityDetailsScreen(
      {super.key, required this.opportunity, this.isReservation = false});

  @override
  _OpportunityManagerScreenState createState() =>
      _OpportunityManagerScreenState();
}

class _OpportunityManagerScreenState extends State<OpportunityDetailsScreen> {
  final ScrollController verticalScrollController = ScrollController();
  final ScrollController horizontalScrollController = ScrollController();
  late Future<User?> _ownerFuture;

  @override
  void initState() {
    super.initState();
    _ownerFuture = Provider.of<UserApiHandler>(context, listen: false)
        .getUserByID(widget.opportunity.userId);
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: FutureBuilder<User?>(
        future: _ownerFuture,
        builder: (context, snapshot) {
          /*
          if (snapshot.connectionState == ConnectionState.waiting) {
            return Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(child: Text('Error: ${snapshot.error}'));
          } else if (!snapshot.hasData || snapshot.data == null) {
            return Center(child: Text('Owner not found'));
          }
          
          final owner = null;//snapshot.data!;
          */
          final opportunity = widget.opportunity;
          final DateTime dateTime = opportunity.date;
          final String formattedTime = DateFormat('HH:mm').format(dateTime);

          return LayoutBuilder(
            builder: (context, constraints) {
              if (constraints.maxWidth < 600) {
                return _buildMobileLayout(opportunity, formattedTime);
              } else if (constraints.maxWidth < 1200) {
                return _buildTabletLayout(opportunity, formattedTime);
              } else {
                return _buildDesktopLayout(opportunity, formattedTime);
              }
            },
          );
        },
      ),
    );
  }

  // Mobile layout (Vertical scroll)
  Widget _buildMobileLayout(Opportunity opportunity, String time) {
    return SingleChildScrollView(
      controller: verticalScrollController,
      padding: const EdgeInsets.all(20.0),
      child: Column(
        children: [
          OpportunityImages(opportunity: opportunity),
          SizedBox(height: 20),
          OpportunityDetails(
            name: opportunity.name,
            description: opportunity.description,
          ),
          SizedBox(height: 20),
          if (!widget.isReservation)
            ReservationButton(
              availableVacancies: 2,
              onPressed: (numberOfPersons) {
                createTempReservation(numberOfPersons);
              },
            ),
          SizedBox(height: 20),
          OpportunityAdditionalInfo(
            price: opportunity.price,
            location: opportunity.location,
            address: opportunity.address,
            vacancies: opportunity.vacancies,
            firstName: "",
            lastName: "",
            time: time,
            date: opportunity.date,
          ),
          SizedBox(height: 20),
          OpportunityLocationMap(
            address: opportunity.address,
          ),
          /*
          //REVIEWS
          SizedBox(height: 20),
          Text(
          'Opiniões',
          style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
        ),
        SizedBox(height: 10),
        ..._reviews.map((review) => ReviewCard(
            rating: review.rating,
            description: review.description ?? '',
            )).toList(),
            */
        ],
      ),
    );
  }

  // Tablet layout (Vertical scroll with Scrollbar)
  Widget _buildTabletLayout(Opportunity opportunity, String time) {
    return Scrollbar(
      thumbVisibility: true,
      controller: verticalScrollController,
      child: SingleChildScrollView(
        controller: verticalScrollController,
        padding: const EdgeInsets.all(20.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Left Column (Image and Details)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityImages(opportunity: opportunity),
                  OpportunityDetails(
                    name: opportunity.name,
                    description: opportunity.description,
                  ),
                  if (!widget.isReservation)
                    ReservationButton(
                      availableVacancies: opportunity.vacancies,
                      onPressed: (numberOfPersons) {
                        createTempReservation(numberOfPersons);
                      },
                    ),
                ],
              ),
            ),
            SizedBox(width: 20), // Add space between the two columns

            // Right Column (Additional Info and Map)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityAdditionalInfo(
                    price: opportunity.price,
                    location: opportunity.location,
                    address: opportunity.address,
                    vacancies: opportunity.vacancies,
                    firstName: "teste",
                    lastName: "teste",
                    time: time,
                    date: opportunity.date,
                  ),
                  SizedBox(height: 20),
                  OpportunityLocationMap(
                    address: opportunity.address,
                  ),
                  /*
                  //REVIEWS
                  SizedBox(height: 20),
                  Text(
                  'Opiniões',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  ..._reviews.map((review) => ReviewCard(
                      rating: review.rating,
                      description: review.description ?? '',
                      )).toList(),
                      */
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  // Desktop layout (Both vertical and horizontal scroll with Scrollbar)
  Widget _buildDesktopLayout(Opportunity opportunity, String time) {
    return Scrollbar(
      thumbVisibility: true,
      controller: verticalScrollController,
      child: SingleChildScrollView(
        controller: verticalScrollController,
        scrollDirection: Axis.vertical,
        padding: const EdgeInsets.all(20.0),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Left column (images and details)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  SizedBox(height: 20),
                  OpportunityImages(opportunity: opportunity),
                  OpportunityDetails(
                    name: opportunity.name,
                    description: opportunity.description,
                  ),
                  SizedBox(height: 20),
                  if (!widget.isReservation)
                    ReservationButton(
                      availableVacancies: widget.opportunity.vacancies,
                      onPressed: (numberOfPersons) {
                        createTempReservation(numberOfPersons);
                        print('Reservation button pressed!');
                      },
                    ),
                ],
              ),
            ),
            SizedBox(width: 20),
            // Right column (additional info and map)
            Expanded(
              flex: 1,
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  OpportunityAdditionalInfo(
                    price: opportunity.price,
                    location: opportunity.location,
                    address: opportunity.address,
                    vacancies: opportunity.vacancies,
                    firstName: "teste",
                    lastName: "teste",
                    time: time,
                    date: opportunity.date,
                  ),
                  SizedBox(height: 20),
                  OpportunityLocationMap(
                    address: opportunity.address,
                  ),
                  /*
                  //REVIEWS
                  SizedBox(height: 20),
                  Text(
                  'Opiniões',
                  style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  ..._reviews.map((review) => ReviewCard(
                      rating: review.rating,
                      description: review.description ?? '',
                      )).toList(),
                      */
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> createCheckoutSessionReservation(Reservation reservation) async {
    String? checkoutId;

    if (reservation != null) {
      checkoutId = await Provider.of<PaymentApiHandler>(context, listen: false)
          .createReservationCheckoutSession(reservation);

      if (checkoutId != null) {
        // Now that we have the sessionId, we need to construct the checkout URL.
        final checkoutUrl = 'https://checkout.stripe.com/pay/$checkoutId';

        // Use url_launcher to open the checkout session in the user's browser.
        if (await canLaunch(checkoutUrl)) {
          await launch(checkoutUrl);
        } else {
          print('Could not launch $checkoutUrl');
        }
      } else {
        print('Failed to create checkout session');
      }
    }
  }

  Future<void> createTempReservation(int numberOfPersons) async {
    User? user = await _getCachedUser();

    // Check if user is null before proceeding
    if (user == null) {
      print('No user found. Cannot create reservation.');
      return null;
    }

    // Create a reservation
    final reservation = Reservation(
        opportunityId: widget.opportunity.opportunityId,
        userId: user.userId,
        checkInDate: widget.opportunity.date,
        numOfPeople: numberOfPersons,
        reservationDate: DateTime.now(),
        isActive: true,
        fixedPrice:
            (widget.opportunity.price * 0.1) + widget.opportunity.price);

    // Save the reservation asynchronously
    await saveReservation(reservation);
    // Create a checkout session for the reservation
    createCheckoutSessionReservation(reservation);
  }

  Future<User?> _getCachedUser() async {
    try {
      // Fetch the user from the API or local storage
      final user = await Provider.of<UserApiHandler>(context, listen: false)
          .getStoredUser();
      return user;
    } catch (e) {
      print('Error fetching user: $e');
      return null;
    }
  }
}
