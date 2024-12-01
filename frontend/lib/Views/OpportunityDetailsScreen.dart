import 'package:flutter/material.dart';
import 'package:frontend/Models/Favorite.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/Models/Review.dart';
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
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:frontend/Services/user_services.dart';
import 'package:frontend/Services/review_api_handler.dart';
import 'package:frontend/Components/ReviewCard.dart';

class OpportunityDetailsScreen extends StatefulWidget {
  final bool isReservable;
  final Opportunity opportunity;

  const OpportunityDetailsScreen(
      {super.key, required this.opportunity, this.isReservable = true});

  @override
  _OpportunityManagerScreenState createState() =>
      _OpportunityManagerScreenState();
}

class _OpportunityManagerScreenState extends State<OpportunityDetailsScreen> {
  final ScrollController verticalScrollController = ScrollController();
  final ScrollController horizontalScrollController = ScrollController();
  late Future<User?> _ownerFuture;
  List<Review> reviews = [];
  bool isLoading = true;
  late bool isOwner;

  @override
  void initState() {
    super.initState();
    isOwner = false;
    _ownerFuture = Provider.of<UserApiHandler>(context, listen: false)
        .getUserByID(widget.opportunity.userId);
    
    if(isOwner == false){
      _checkUserOwnership();
    }
    fetchReviews();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: FutureBuilder<User?>(
        future: _ownerFuture,
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return Center(child: CircularProgressIndicator());
          }
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
          DynamicActionButton(
            text: "Adicionar aos Favoritos",
            color: Color(0xFF50C878),
            icon: Icons.star,
            onPressed: () {
              addToFavorites(context, widget.opportunity.opportunityId);
            },
          ),
          SizedBox(height: 20),
          if (widget.isReservable && !isOwner && widget.opportunity.isActive)
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
          // REVIEWS
          SizedBox(height: 20),
          Text(
            'Opiniões',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 10),
          // Check if reviews are loading
          isLoading
              ? Center(child: CircularProgressIndicator())
              : reviews.isEmpty
                  ? Text('Nenhuma opinião disponível.')
                  : Column(
                      children: reviews.map((review) {
                        return ReviewCard(
                          rating: review.rating,
                          description: review.description ?? '',
                        );
                      }).toList(),
                    ),
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
                  SizedBox(height: 20),
                  DynamicActionButton(
                    text: "Adicionar aos Favoritos",
                    color: Color(0xFF50C878),
                    icon: Icons.star,
                    onPressed: () {
                      addToFavorites(context, widget.opportunity.opportunityId);
                    },
                  ),
                  if (widget.isReservable && !isOwner && widget.opportunity.isActive)
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

            // Right Column (Additional Info, Map, and Reviews)
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
                  // REVIEWS
                  SizedBox(height: 20),
                  Text(
                    'Opiniões',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  // Check if reviews are loading
                  isLoading
                      ? Center(child: CircularProgressIndicator())
                      : reviews.isEmpty
                          ? Text('Nenhuma opinião disponível.')
                          : Column(
                              children: reviews.map((review) {
                                return ReviewCard(
                                  rating: review.rating,
                                  description: review.description ?? '',
                                );
                              }).toList(),
                            ),
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
                  DynamicActionButton(
                    text: "Adicionar aos Favoritos",
                    color: Color(0xFF50C878),
                    icon: Icons.star,
                    onPressed: () {
                      addToFavorites(context, widget.opportunity.opportunityId);
                    },
                  ),
                  SizedBox(height: 20),
                  if (widget.isReservable && !isOwner && widget.opportunity.isActive)
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
            // Right column (additional info, map, and reviews)
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
                  // REVIEWS
                  SizedBox(height: 20),
                  Text(
                    'Opiniões',
                    style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: 10),
                  // Check if reviews are loading
                  isLoading
                      ? Center(child: CircularProgressIndicator())
                      : reviews.isEmpty
                          ? Text('Nenhuma opinião disponível.')
                          : Column(
                              children: reviews.map((review) {
                                return ReviewCard(
                                  rating: review.rating,
                                  description: review.description ?? '',
                                );
                              }).toList(),
                            ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }


  

  Future<void> createCheckoutSessionReservation(Reservation reservation) async {
    if (reservation != null) {
      final sessionUrl = await Provider.of<PaymentApiHandler>(context, listen: false)
          .createReservationCheckoutSession(reservation);

      if (sessionUrl != null) {
        // Use url_launcher to open the Stripe Checkout session
        if (await canLaunch(sessionUrl)) {
          await launch(sessionUrl);
        } else {
          print('Could not launch $sessionUrl');
        }
      } else {
        print('Failed to create checkout session');
      }
    }
  }

  Future<void> createTempReservation(int numberOfPersons) async {
    User? user = await UserServices.getCachedUser(context);

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

    await saveReservation(reservation);
    createCheckoutSessionReservation(reservation);
  }

  Future<void> _checkUserOwnership() async {
    bool owner = await _isUserOwner();
    setState(() {
      isOwner = owner;
    });
  }

  Future<bool> _isUserOwner() async {
    try {
      User? loggedInUser = await UserServices.getCachedUser(context);
      return loggedInUser?.userId == widget.opportunity.userId;
    } catch (e) {
      print('Error checking user ownership: $e');
      return false;
    }
  }

  /// Documentation for addToFavortites
  /// @param: BuildContext context
  /// @param: int oppId Opportunity id
  /// This Function adds an Opportunity to a user's Favorites
  static Future<void> addToFavorites(BuildContext context, int oppId) async {
    User? user = await UserServices.getCachedUser(context);

    if (!context.mounted) return; // Check if the context is still valid

    if (user != null) {
      final favorite = Favorite(userId: user.userId, opportunityId: oppId);
      final Favorite? addedFavorite = await Provider.of<UserApiHandler>(context, listen: false)
          .addFavorite(favorite);

      if (!context.mounted) return;

      if (addedFavorite != null) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Oportunidade adicionada aos seus Favoritos!'),
            backgroundColor: Colors.green,
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text(
                'Erro ao adicionar aos seus Favoritos, a Oportunidade já existe na sua lista ou tente novamente mais tarde.'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  Future<void> fetchReviews() async {
    setState(() {
      isLoading = true; // Ensure loading state is set before fetching.
    });

    try {
      final List<Review>? fetchedReviews = await Provider.of<ReviewApiHandler>(
        context,
        listen: false,
      ).getReviewsByOppId(widget.opportunity.opportunityId);

      setState(() {
        reviews = fetchedReviews ?? []; // Assign an empty list if null.
        isLoading = false; // Stop loading after data fetch.
      });
    } catch (error) {
      setState(() {
        isLoading = false; // Stop loading after error.
        reviews = []; // Clear reviews or handle errors more specifically.
      });
      // Log or handle the error
      print('Failed to load reviews: $error');
    }
  }
}
